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

        private void Awake()
        {
            // 注册事件
            ServiceLocator.Get<IMemoryMonitor>().Register(this);
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
        /// AB包文件自定义后缀
        /// </summary>
        public string AbSuffix => ".assetbundle";

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>是否初始化成功</returns>
        public async Task Init()
        {
            // 构建主包信息
            _mainWrapper = new BundleWrapper(AbMainName, PathUtility.GetAbLoadPath(AbMainName + AbSuffix));
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

            // 构建全部AB包信息
            var abNames = _abManifest.GetAllAssetBundles();
            foreach (var abName in abNames)
            {
                // 初始化包装器
                _nameToWrapperMap.TryAdd(abName, new BundleWrapper(abName.ToLower(), PathUtility.GetAbLoadPath($"{abName}{AbSuffix}")));
            }
        }

        /// <summary>
        /// 异步加载指定AB包
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<AssetBundle> LoadBundleAsync(EAssetBundleType assetBundleType, CancellationToken token = default)
        {
            var abName = assetBundleType.ToString().ToLower();
            
            // 先检查未使用缓存是否有
            if (_nameToNonRefWrapperMap.TryGetValue(abName, out var unUserWrapper))
            {
                _nameToWrapperMap.Add(abName, unUserWrapper);
                _nameToNonRefWrapperMap.Remove(abName);
            }
            
            if (!_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                LogManager.LogError($"AB包：{assetBundleType}不存在");
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

        /// <summary>
        /// 异步卸载指定AB包
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="unloadAllLoadedObjects"></param>
        /// <returns></returns>
        public void UnloadBundleAsync(EAssetBundleType assetBundleType, bool unloadAllLoadedObjects = false)
        {
            var abName = assetBundleType.ToString().ToLower();
            if (_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                PushUnUseBundle(wrapper);
                _nameToWrapperMap.Remove(abName);
            }
        }

        /// <summary>
        /// 缓存未使用的包
        /// </summary>
        /// <param name="bundleWrapper"></param>
        private void PushUnUseBundle(BundleWrapper bundleWrapper)
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
            foreach (var bundleWrapper in _nameToWrapperMap.Values)
            {
                if (bundleWrapper.RefCount == 0)
                {
                    await bundleWrapper.UnloadAsync(unloadAllObjects);
                }
                else
                {
                    LogManager.LogWarning($"{bundleWrapper.BundelName}包，剩余引用计数：{bundleWrapper.RefCount}，无法卸载");
                }
            }
            
            // 清空缓存
            _nameToWrapperMap.Clear();
            // 卸载主包
            await _mainWrapper.UnloadAsync(unloadAllObjects);
            // 卸载所有AB包
            AssetBundle.UnloadAllAssetBundles(unloadAllObjects);
            _mainWrapper = null;
            _abManifest = null;
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
            
            await unUsebundleWrapper?.UnloadAsync(true);
        }

        protected override void OnDestroy()
        {
            // 注销事件
            ServiceLocator.Get<IMemoryMonitor>().Unregister(this);
        }
    }
}
