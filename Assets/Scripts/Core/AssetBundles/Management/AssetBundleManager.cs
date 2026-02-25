using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.Systems.Memorys;
using Core.Tasks.Extensions;
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
        // 主包信息
        private BundleWrapper _mainWrapper;
        // 主包清单信息
        private AssetBundleManifest _abManifest;
        // 内存监听器
        private IMemoryMonitor _memoryMonitor;

        private void Awake()
        {
            _memoryMonitor = ServiceLocator.Get<IMemoryMonitor>();
            // 注册事件
            _memoryMonitor.Register(this);
        }

        /// <summary>
        /// 获取AssetBundle主包名
        /// </summary>
        /// <value>
        /// 不同平台对应的主包名
        /// </value>
        /// <remarks>
        /// 由运行时的平台决定，支持PC、Android、IOS
        /// 不同平台需实现不同的返回名称，否则返回null
        /// </remarks>
        private static string AbMainName
        {
            get
            {
#if UNITY_ANDROID
                    return "Android";
#elif UNITY_IOS
                    return "IOS";
#elif UNITY_STANDALONE_WIN
                return "PC";
#else
                    LogManager.LogError("未实现该平台的主包名");
                    return null;
#endif
            }
        }

        /// <summary>
        /// 初始化默认包
        /// 更新使用
        /// </summary>
        /// <param name="defaultName"></param>
        public async Task InitDefault(string defaultName)
        {
            // 构建主包信息
            _mainWrapper = new BundleWrapper(AbMainName, PathUtility.GetAbLoadPath($"{AbMainName}{FileUtility.AbSuffix}"), this);
            // 加载主包
            await _mainWrapper.LoadFromFileAsync();

            // 加载依赖文件
            _abManifest = await _mainWrapper.AssetBundle
                .LoadAssetAsync<AssetBundleManifest>(nameof(AssetBundleManifest)).ToTask<AssetBundleManifest>();
            if (_abManifest == null)
            {
                LogManager.LogError($"主包依赖文件加载失败");
                return;
            }
            
            _nameToWrapperMap.TryAdd(defaultName, new BundleWrapper(defaultName, PathUtility.GetAbLoadPath($"{defaultName}{FileUtility.AbSuffix}"), this));
        }
        
        public async Task Init()
        {
            // 先卸载原来的默认包
            await UnloadAllBundles(false);
            
            // 构建主包信息
            _mainWrapper = new BundleWrapper(AbMainName, PathUtility.GetAbLoadPath($"{AbMainName}{FileUtility.AbSuffix}"), this);
            // 加载主包
            await _mainWrapper.LoadFromFileAsync();

            // 加载依赖文件
            _abManifest = await _mainWrapper.AssetBundle.LoadAssetAsync<AssetBundleManifest>(nameof(AssetBundleManifest)).ToTask<AssetBundleManifest>();
            if (!_abManifest)
            {
                LogManager.LogError($"主包依赖文件加载失败");
                return;
            }

            // 构建全部AB包信息
            var abNames = _abManifest.GetAllAssetBundles();
            foreach (var abName in abNames)
            {
                // 初始化包装器
                _nameToWrapperMap.TryAdd(abName, new BundleWrapper(abName.ToLower(), PathUtility.GetAbLoadPath($"{abName}{FileUtility.AbSuffix}"), this));
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
            var dependencies = _abManifest.GetAllDependencies(abName);
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
                if (bundleWrapper.RefCount == 0)
                {
                    await bundleWrapper.TryUnloadAsync(false);
                }
                else
                {
                    LogManager.LogWarning($"{bundleWrapper.BundelName}包，剩余引用计数：{bundleWrapper.RefCount}，无法卸载");
                }
            }
            
            // 清空缓存
            _nameToWrapperMap.Clear();
            _nameToNonRefWrapperMap.Clear();
            // 卸载依赖文件
            _mainWrapper.Unload();
            _abManifest = null;
            // 卸载主包
            await _mainWrapper.TryUnloadAsync(unloadAllObjects);
            _mainWrapper = null;
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
            
            await unUsebundleWrapper?.TryUnloadAsync(false);
        }

        protected override void OnDestroy()
        {
            // 注销事件
            _memoryMonitor.Unregister(this);
            _memoryMonitor = null;
        }
    }
}
