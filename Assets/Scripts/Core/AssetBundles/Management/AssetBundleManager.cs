using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.DI;
using Core.Serialize.Json;
using Core.Systems.Memorys;
using Core.Utility;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// AB包管理器
    /// </summary>
    internal class AssetBundleManager : IAssetBundleManager
    {
        private readonly IJsonManager _jsonManager;
        // 缓存包包装器，便于查找
        private readonly Dictionary<string, BundleWrapper> _nameToWrapperMap = new();
        // 加载任务缓存，防并发
        private readonly Dictionary<string, Task<BundleWrapper>> _bundleLoadingTasks = new();
        // 热包列表
        private readonly List<BundleWrapper> _hotBundles = new();
        // 冷包列表
        private readonly List<BundleWrapper> _coldBundles = new();
        // 临界活跃数，高于该数值则放入热包列表，小于则放入冷包列表
        private readonly int _criticalActiveCount;
        // 单个AB包滑动窗口最大数
        private readonly int _bundleSlidingWindowMaxCount;
        // 单个滑动窗口最大时间
        private readonly float _maxDurationPerWindow;
        
        public AssetCatalog Catalog { get; private set; }
        
        private AssetBundleManager(int criticalActiveCount, int bundleSlidingWindowMaxCount, float maxDurationPerWindow, 
            IMemoryMonitor memoryMonitor, IJsonManager jsonManager)
        {
            // 注册事件
            memoryMonitor.Register(this);
            _jsonManager = jsonManager;
            _criticalActiveCount = criticalActiveCount;
            _bundleSlidingWindowMaxCount = bundleSlidingWindowMaxCount;
            _maxDurationPerWindow = maxDurationPerWindow;
        }
        
        public async Task Init()
        {
            // 读取本地清单文件
            Catalog = await _jsonManager.FromJsonAsync<AssetCatalog>(
                PathUtility.GetAbLoadPath(FileUtility.CatalogDefaultName),
                settings: NewtonsoftJsonUtility.SerializerSettings);
            // 构建全部AB包信息
            foreach (var abPackageInfo in Catalog.ABPackageCollection.Values)
            {
                var abName = abPackageInfo.Name;
                var loadPath = PathUtility.GetAbLoadPath(abPackageInfo.Name.WithAbSuffix());
                // 初始化包装器
                var window = DIContainer.Create<LFUSlidingWindow>(parameterValues: new object[] { _bundleSlidingWindowMaxCount, _maxDurationPerWindow });
                var bundleWrapper = DIContainer.Create<BundleWrapper>(parameterValues: new object[] { abName, loadPath, this, window });
                bundleWrapper.OnAccessAsset += UpdateBundleCacheState;
                _nameToWrapperMap.TryAdd(abName, bundleWrapper);
            }
        }
        
        public BundleWrapper LoadBundle(string abName)
        {
            if (!_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                throw new KeyNotFoundException($"{nameof(AssetBundleManager)}: {abName} key is not found");
            }

            // 加载依赖和目标AB包
            LoadDependenciesAndTarget(abName);
            // 返回指定AB包
            return wrapper;
        }
        
        /// <summary>
        /// 异步加载依赖包和目标包
        /// </summary>
        /// <param name="abName">AB包名称（不含拓展名）</param>
        /// <returns></returns>
        private void LoadDependenciesAndTarget(string abName)
        {
            // 获取该AB包的所有依赖
            var dependencies = Catalog.ABPackageCollection.GetAllDependencies(abName);
            // 加载所有依赖包
            foreach (var dependency in dependencies)
            {
                var wrapper = _nameToWrapperMap[dependency];
                wrapper.IsActive = true;
                wrapper.LoadFromFile();
                wrapper.Retain();
                Logger.Log($"{nameof(AssetBundleManager)}: '{abName}' assetBundle dependency '{dependency}' will be loaded");
            }

            // 加载目标包
            _nameToWrapperMap[abName].LoadFromFile();
        }
        
        public async Task<BundleWrapper> LoadBundleAsync(string abName, CancellationToken token = default)
        {
            if (!_nameToWrapperMap.ContainsKey(abName))
                throw new KeyNotFoundException($"[{nameof(AssetBundleManager)}]: {abName} assetBundle key is not found");

            // 已存在同名加载任务，直接复用
            if (_bundleLoadingTasks.TryGetValue(abName, out var existingTask))
                return await existingTask;
            
            // 异步加载AB包及其依赖
            var task = LoadBundleInternalAsync(abName, token);
            // 缓存当前正在加载的任务
            if (!_bundleLoadingTasks.TryAdd(abName, task))
            {
                // 并发极端情况，已添加，返回同一个任务
                task = _bundleLoadingTasks[abName];
            }
            
            try
            {
                return await task;
            }
            finally
            {
                _bundleLoadingTasks.Remove(abName);
            }
        }
        
        /// <summary>
        /// 异步加载AB包（内部）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<BundleWrapper> LoadBundleInternalAsync(string abName, CancellationToken token)
        {
            await LoadDependenciesAndTargetAsync(abName, token);
            return _nameToWrapperMap[abName];
        }
        
        /// <summary>
        /// 异步加载依赖包和目标包
        /// </summary>
        /// <param name="abName">AB包名称（不含拓展名）</param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task LoadDependenciesAndTargetAsync(string abName, CancellationToken token)
        {
            // 获取该AB包的所有依赖
            var dependencies = Catalog.ABPackageCollection.GetAllDependencies(abName);
            // 并发加载依赖，并存储每个加载任务及其对应的依赖包名
            var dependenciesTasks = new Dictionary<string, Task<bool>>(dependencies.Length);
            foreach (var dependency in dependencies)
            {
                var wrapper = _nameToWrapperMap[dependency];
                wrapper.IsActive = true;
                dependenciesTasks.Add(dependency, wrapper.LoadFromFileAsync(token));
                //Logger.Log($"{nameof(AssetBundleManager)}: '{abName}' assetBundle dependency '{dependency}' will be loaded");
            }

            // 等待所有依赖加载完毕
            await Task.WhenAll(dependenciesTasks.Values);
            // 仅对加载成功的依赖增加引用计数
            foreach (var (depName, task) in dependenciesTasks)
            {
                // 已完成直接取 Result 即可
                if (task.Result)
                {
                    _nameToWrapperMap[depName].Retain();
                }
            }

            var isSuccess = false;
            try
            {
                // 加载目标包
                isSuccess = await _nameToWrapperMap[abName].LoadFromFileAsync(token);
            }
            finally
            {
                // 加载目标包失败
                if (!isSuccess)
                {
                    // 只回滚加载成功的依赖（它们的引用计数被增加了）
                    foreach (var (depName, task) in dependenciesTasks)
                    {
                        if (task.Result)
                        {
                            _nameToWrapperMap[depName].Release();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新AB包缓存状态
        /// </summary>
        private void UpdateBundleCacheState(BundleWrapper bundleWrapper)
        {
            if (_hotBundles.Contains(bundleWrapper) && bundleWrapper.AccessCount < _criticalActiveCount)
            {
                _hotBundles.Remove(bundleWrapper);
                _coldBundles.Add(bundleWrapper);
            }
            else if (_coldBundles.Contains(bundleWrapper) && bundleWrapper.AccessCount >= _criticalActiveCount)
            {
                _coldBundles.Remove(bundleWrapper);
                _hotBundles.Add(bundleWrapper);
            }
            // 第一次加载该包，默认放入冷列表
            else if(!_coldBundles.Contains(bundleWrapper))
            {
                _coldBundles.Add(bundleWrapper);
            }
        }

        public void ReleaseDependencies(string abName)
        {
            var dependencies = Catalog.ABPackageCollection.GetAllDependencies(abName);
            foreach (var dependency in dependencies)
            {
                var wrapper = _nameToWrapperMap[dependency];
                if (wrapper.IsActive)
                {
                    wrapper.Release();
                }
            }
        }
        
        public async Task UnloadAllBundles(bool unloadAllObjects)
        {
            _hotBundles.Clear();
            _coldBundles.Clear();
            
            var unloads = new List<Task>(_nameToWrapperMap.Values.Count);
            foreach (var bundleWrapper in _nameToWrapperMap.Values)
            {
                unloads.Add(bundleWrapper.TryUnloadAsync(unloadAllObjects));
            }
            // 等到所有包卸载完成
            await Task.WhenAll(unloads);
            
            // 卸载所有AB包
            AssetBundle.UnloadAllAssetBundles(unloadAllObjects);
            GC.Collect();
        }
        
        public async void OnReport()
        {
            try
            {
                // LRU + LFU
                var bundles = _coldBundles.Count > 0 ? _coldBundles : _hotBundles.Count > 0 ? _hotBundles : null;
                if (bundles == null)
                    return;
                
                BundleWrapper unUseBundleWrapper = null;
                foreach (var bundleWrapper in bundles)
                {
                    // 跳过活跃包（正在被使用的）
                    if (bundleWrapper.IsActive)
                        continue;
                    
                    // 最久没使用
                    if (unUseBundleWrapper == null || unUseBundleWrapper.LastAccessTime > bundleWrapper.LastAccessTime)
                    {
                        unUseBundleWrapper = bundleWrapper;
                    }
                }

                if (unUseBundleWrapper == null)
                {
                    // 降级处理
                    // 选最久未访问的活跃包
                    foreach (var bundleWrapper in bundles)
                    {
                        if (unUseBundleWrapper == null || unUseBundleWrapper.LastAccessTime > bundleWrapper.LastAccessTime)
                        {
                            unUseBundleWrapper = bundleWrapper;
                        }
                    }
                }
                
                // 卸载包
                if (unUseBundleWrapper != null)
                {
                    await unUseBundleWrapper.TryUnloadAsync(true);
                    // 从列表中移除
                    bundles.Remove(unUseBundleWrapper);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(AssetBundleManager)}: Unload AssetBundle exception,{e.Message}");
            }
        }
    }
}
