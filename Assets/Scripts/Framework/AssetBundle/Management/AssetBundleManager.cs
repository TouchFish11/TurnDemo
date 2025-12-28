using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using XLua;

namespace Framework
{
    /// <summary>
    /// AB包管理器
    /// </summary>
    [LuaCallCSharp]
    public class AssetBundleManager : SingletonBase<AssetBundleManager>, IAssetBundleManager
    {
        // 缓存全部包加载器
        private readonly Dictionary<string, BundleWrapper> _nameToWrapperMap = new Dictionary<string, BundleWrapper>();
        // 主包信息
        private BundleWrapper _mainWrapper;
        // 主包清单信息
        private AssetBundleManifest _abManifest;

        private AssetBundleManager()
        {

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
        private string AbMainName
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
                    DebugMgr.LogError("未实现该平台的主包名");
                    return null;
#endif
            }
        }

        /// <summary>
        /// AB包文件自定义后缀
        /// </summary>
        public string AbSuffix { get; } = ".assetbundle";

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>是否初始化成功</returns>
        public async Task<bool> Init()
        {
            // 清空缓存
            ClearCache();

            // 构建主包信息
            _mainWrapper = new AssetBundleWrapper(AbMainName, PathUtility.GetAbLoadPath(AbMainName + AbSuffix));
            // 加载主包
            bool isSuccess = await _mainWrapper.LoadFromFileAsync();
            if(!isSuccess)
            {
                return false;
            }

            // 加载依赖文件
            _abManifest = await _mainWrapper.Convert<AssetBundleWrapper>().LoadAssetAsync<AssetBundleManifest>(nameof(AssetBundleManifest));
            if (_abManifest == null)
            {
                return false;
            }

            // 构建全部AB包信息
            string[] abNames = _abManifest.GetAllAssetBundles();
            for (int i = 0; i < abNames.Length; i++)
            {
                // 没用即添加，有即比较MD5是否相同：不同则替换，同则不处理
                string abName = abNames[i].ToLower();
                // 初始化包装器
                _nameToWrapperMap.TryAdd(abName, new AssetBundleWrapper(abName, PathUtility.GetAbLoadPath(abName + AbSuffix)));
            }

            return true;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetBundleType">资源所在AB包名</param>
        /// <param name="assetName">资源名</param>
        public async Task<T> LoadAssetAsync<T>(E_AssetBundleType assetBundleType, string assetName) where T : Object
        {
            string abName = assetBundleType.ToString().ToLower();
            if (!_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                LogManager.LogError($"AB包：{abName}不存在");
                return null;
            }

            // 存在资源缓存，直接返回
            if(wrapper.Convert<AssetBundleWrapper>().TryGetAsset(assetName, out Object asset))
            {
                return asset as T;
            }

            // 加载依赖和目标AB包
            await LoadDependenciesAndTargetAsync(abName);
            // 加载资源
            return await wrapper.Convert<AssetBundleWrapper>().LoadAssetAsync<T>(assetName);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetBundleType">资源所在AB包名</param>
        /// <param name="assetName">资源名</param>
        public async Task<Object> LoadAssetAsync(E_AssetBundleType assetBundleType, string assetName, System.Type type)
        {
            string abName = assetBundleType.ToString().ToLower();
            if (!_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                LogManager.LogError($"AB包：{abName}不存在");
                return null;
            }

            // 存在资源缓存，直接返回
            if (wrapper.Convert<AssetBundleWrapper>().TryGetAsset(assetName, out Object asset))
            {
                return asset;
            }

            // 加载依赖和目标AB包
            await LoadDependenciesAndTargetAsync(abName);
            // 加载资源
            return await wrapper.Convert<AssetBundleWrapper>().LoadAssetAsync(assetName, type);
        }

        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async Task<T[]> LoadAssetsAsync<T>(E_AssetBundleType assetBundleType) where T : Object
        {
            string abName = assetBundleType.ToString().ToLower();
            if (!_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                LogManager.LogError($"AB包：{abName}不存在");
                return new T[0];
            }

            // 存在资源缓存，直接返回
            T[] assets = wrapper.Convert<AssetBundleWrapper>().GetAssets<T>();
            if (assets.Length > 0)
            {
                return assets;
            }

            // 加载依赖和目标AB包
            await LoadDependenciesAndTargetAsync(abName);
            // 加载所有资源
            return await wrapper.Convert<AssetBundleWrapper>().LoadAllAssetsAsync<T>();
        }

        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async Task<Object[]> LoadAssetsAsync(E_AssetBundleType assetBundleType, System.Type type)
        {
            string abName = assetBundleType.ToString().ToLower();
            if (!_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                LogManager.LogError($"AB包：{abName}不存在");
                return new Object[0];
            }

            // 存在资源缓存，直接返回
            Object[] assets = wrapper.Convert<AssetBundleWrapper>().GetAssets();
            if (assets.Length > 0)
            {
                return assets;
            }

            // 加载依赖和目标AB包
            await LoadDependenciesAndTargetAsync(abName);
            // 加载所有资源
            return await wrapper.Convert<AssetBundleWrapper>().LoadAllAssetsAsync(type);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetBundleType">AB包类型</param>
        /// <param name="assetName">卸载的资源名称</param>
        public void UnloadAsset(E_AssetBundleType assetBundleType, string assetName)
        {
            string abName = assetBundleType.ToString().ToLower();
            if (_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                // 卸载资源
                wrapper.Convert<AssetBundleWrapper>().UnloadAsset(assetName);
            }
        }

        /// <summary>
        /// 异步卸载包
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="unloadAllLoadedObjects"></param>
        /// <returns></returns>
        public async Task<bool> UnloadBundleAsync(E_AssetBundleType assetBundleType, bool unloadAllLoadedObjects = false)
        {
            string abName = assetBundleType.ToString().ToLower();
            if (_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                return await wrapper.UnloadAsync(unloadAllLoadedObjects);
            }

            // 包不存在，视为卸载成功
            return true;
        }

        /// <summary>
        /// 异步加载场景包
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadSceneBundleAsync()
        {
            string abName = E_AssetBundleType.Scene.ToString().ToLower();
            if (_nameToWrapperMap.TryGetValue(abName, out var _))
            {
                return await LoadDependenciesAndTargetAsync(abName);
            }

            // 包不存在，视为加载失败
            return false;
        }

        /// <summary>
        /// 是否包含该场景路径
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public bool ContainPath(string scenePath)
        {
            string abName = E_AssetBundleType.Scene.ToString().ToLower();
            if (_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                return wrapper.Convert<SceneBundleWrapper>().ContainPath(scenePath);
            }
            return false;
        }

        /// <summary>
        /// 获取所有的场景路径
        /// </summary>
        public async Task<string[]> GetAllScenePaths()
        {
            string abName = E_AssetBundleType.Scene.ToString().ToLower();
            if (_nameToWrapperMap.TryGetValue(abName, out var wrapper))
            {
                // 加载依赖和目标包
                bool isSuccess = await LoadDependenciesAndTargetAsync(abName);
                if(!isSuccess)
                {
                    LogManager.LogError($"场景包加载失败，无法获取场景路径");
                    return new string[0];
                }
                return wrapper.Convert<SceneBundleWrapper>().GetAllScenePaths();
            }

            // 包不存在，返回空数组
            return new string[0];
        }

        /// <summary>
        /// 清空包加载管理器缓存
        /// </summary>
        public void ClearCache()
        {
            foreach (BundleWrapper wrapper in _nameToWrapperMap.Values)
            {
                wrapper.UnloadAsync(true);
            }
            _nameToWrapperMap.Clear();
            _mainWrapper = null;
            _abManifest = null;
            BundleWrapper.UnloadAllAssetBundles(false);
            System.GC.Collect();
        }

        /// <summary>
        /// 异步加载依赖包和目标包
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private async Task<bool> LoadDependenciesAndTargetAsync(string abName)
        {
            // 获取该AB包的所有依赖
            string[] dependencies = _abManifest.GetAllDependencies(abName);
            // 加载所有依赖包
            for (int i = 0; i < dependencies.Length; i++)
            {
                AssetBundleWrapper wrapper = _nameToWrapperMap[dependencies[i]].Convert<AssetBundleWrapper>();
                bool isSuccess = await wrapper.LoadFromFileAsync();
                if (!isSuccess)
                {
                    LogManager.LogError($"依赖包：{dependencies[i]}加载失败，无法加载目标包：{abName}");
                    return isSuccess;
                }
            }

            // 加载目标包
            return await _nameToWrapperMap[abName].LoadFromFileAsync();
        }
    }
}
