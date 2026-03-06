using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Collection;
using Core.Log;
using Core.Serialize.Json;
using Core.Service;
using Core.Singleton;
using Core.Systems.Memorys;
using Core.Utility;
using UnityEngine;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// AB包管理器
    /// </summary>
    public class AssetBundleManager : SingletonAutoMono<AssetBundleManager>, IAssetBundleManager
    {
        // 缓存活跃的包包装器
        private readonly Dictionary<string, BundleWrapper> _nameToWrapperMap = new();
        // 未被引用的包装器缓存
        private readonly Dictionary<string, BundleWrapper> _nameToNonRefWrapperMap = new();
        // 内存监听器
        private IMemoryMonitor _memoryMonitor;
        // 清单文件集合
        private ABPackageCollection _abPackageCollection;

        private void Awake()
        {
            _memoryMonitor = ServiceLocator.Get<IMemoryMonitor>();
            // 注册事件
            _memoryMonitor.Register(this);
        }

        /// <summary>
        /// 初始化默认包
        /// 更新使用
        /// </summary>
        /// <param name="defaultName"></param>
        public async Task InitDefault(string defaultName)
        {
            // 读取本地清单文件
            _abPackageCollection = await ServiceLocator.Get<IJsonManager>().FromJsonAsync<ABPackageCollection>(PathUtility.GetAbLoadPath(FileUtility.ListFileDefaultName));
            if(_abPackageCollection.TryGetValue(defaultName, out var defaultPackage))
            {
                _nameToWrapperMap.TryAdd(defaultName, new BundleWrapper(defaultName, PathUtility.GetAbLoadPath(defaultPackage.Name), this));
            }
        }
        
        public async Task Init()
        {
            // 先卸载原来的默认包
            await UnloadAllBundles(false);
            
            // 读取本地清单文件
            _abPackageCollection = await ServiceLocator.Get<IJsonManager>().FromJsonAsync<ABPackageCollection>(PathUtility.GetAbLoadPath(FileUtility.ListFileDefaultName));
            // 构建全部AB包信息
            foreach (var abPackageInfo in _abPackageCollection.Values)
            {
                var abName = abPackageInfo.Name.Substring(0, abPackageInfo.Name.LastIndexOf('.'));
                // 初始化包装器
                _nameToWrapperMap.TryAdd(abName, new BundleWrapper(abName, PathUtility.GetAbLoadPath(abPackageInfo.Name), this));
            }
        }

        /// <summary>
        /// 异步加载指定AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<AssetBundle> LoadBundleAsync(string abName, CancellationToken token = default)
        {
            // 先检查未使用缓存是否有
            if (_nameToNonRefWrapperMap.TryGetValue(abName, out var unUserWrapper))
            {
                _nameToWrapperMap.Add(abName, unUserWrapper);
                _nameToNonRefWrapperMap.Remove(abName);
            }
            
            if (!_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                LogManager.LogError($"AB包：{abName}不存在");
                return null;
            }

            // 加载依赖和目标AB包
            await LoadDependenciesAndTargetAsync(abName, token);
            // 返回指定AB包
            return wrapper.AssetBundle;
        }

        /// <summary>
        /// 异步加载依赖包和目标包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task LoadDependenciesAndTargetAsync(string abName, CancellationToken token)
        {
            // 获取该AB包的所有依赖
            var dependencies = _abPackageCollection.GetAllDependencies(abName);
            // 加载所有依赖包
            foreach (var dependency in dependencies)
            {
                // 先检查未使用缓存是否有
                if (_nameToNonRefWrapperMap.TryGetValue(dependency, out var unUserWrapper))
                {
                    _nameToWrapperMap.Add(dependency, unUserWrapper);
                    _nameToNonRefWrapperMap.Remove(dependency);
                }
                
                var wrapper = _nameToWrapperMap[dependency];
                await wrapper.LoadFromFileAsync(token);
                LogManager.Log($"{abName}包依赖项：{dependency}，已加载");
            }

            // 加载目标包
            await _nameToWrapperMap[abName].LoadFromFileAsync(token);
        }
        
        public void UnloadBundle(string abName, bool unloadAllLoadedObjects = false)
        {
            if (_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                wrapper.Unload();
            }
        }
        
        public async Task ForceUnloadUnuseBundle()
        {
            foreach (var bundleWrapper in _nameToNonRefWrapperMap.Values)
            {
                await bundleWrapper.TryUnloadAsync(false);
            }
        }

        /// <summary>
        /// 缓存未使用的包
        /// </summary>
        /// <param name="bundleWrapper"></param>
        public void PushUnUseBundle(BundleWrapper bundleWrapper)
        {
            _nameToNonRefWrapperMap.Add(bundleWrapper.BundelName, bundleWrapper);
        }

        /// <summary>
        /// 卸载所有已加载的AssetBundle
        /// 调用该方法后，若需要加载AB包，需重新初始化（Init）管理器
        /// </summary>
        /// <param name="unloadAllObjects"></param>
        public async Task UnloadAllBundles(bool unloadAllObjects)
        {
            // 先释放未使用的AB包
            await ForceUnloadUnuseBundle();
            
            foreach (var bundleWrapper in _nameToWrapperMap.Values)
            {
                await bundleWrapper.TryUnloadAsync(unloadAllObjects);
                if (unloadAllObjects)
                {
                    if (bundleWrapper.RefCount != 0)
                    {
                        LogManager.LogWarning($"{bundleWrapper.BundelName}包和已加载资源已卸载，剩余引用计数：{bundleWrapper.RefCount}，可能导致引用丢失");
                    }
                }
                else
                {
                    LogManager.Log($"{bundleWrapper.BundelName}包已卸载，剩余引用计数：{bundleWrapper.RefCount}");
                }
            }
            
            // 清空缓存
            _nameToWrapperMap.Clear();
            _nameToNonRefWrapperMap.Clear();
            // 置空清单集合
            _abPackageCollection = null;
            // 卸载所有AB包
            AssetBundle.UnloadAllAssetBundles(unloadAllObjects);
            GC.Collect();
        }
        
        public async void OnReport()
        {
            // LRU
            BundleWrapper unUsebundleWrapper = null;
            foreach (var bundleWrapper in _nameToNonRefWrapperMap.Values)
            {
                if (unUsebundleWrapper == null || unUsebundleWrapper.LastUseTime > bundleWrapper.LastUseTime)
                {
                    unUsebundleWrapper = bundleWrapper;
                }
            }

            if (unUsebundleWrapper != null)
            {
                await unUsebundleWrapper.TryUnloadAsync(false);
            }
        }

        protected override void OnDestroy()
        {
            // 注销事件
            _memoryMonitor.Unregister(this);
            _memoryMonitor = null;
        }
    }
}
