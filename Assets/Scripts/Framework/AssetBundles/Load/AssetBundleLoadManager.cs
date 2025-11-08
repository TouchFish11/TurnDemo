using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// AssetBundle资源加载管理器
    /// </summary>
    public sealed class AssetBundleLoadManager : SingletonAutoMono<AssetBundleLoadManager>
    {
        ///存储所有包加载器  Key：包名，Value：包加载器接口
        private readonly Dictionary<string, IBundleLoader> _bundleLoaderDic = new Dictionary<string, IBundleLoader>();
        //主包单独存储
        private AssetBundleLoader _mainAssetBundleInfo;
        //依赖信息文件
        private AssetBundleManifest _abManifest;

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
        public const string AbSuffix = ".assetbundle";

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>是否初始化成功</returns>
        public bool Init()
        {
            //构建主包信息
            _mainAssetBundleInfo = new AssetBundleLoader(AbMainName, PathManager.GetAbLoadPath(AbMainName + AbSuffix));
            //同步加载主包
            if (!_mainAssetBundleInfo.LoadBundle())
            {
                return false;
            }

            //同步加载依赖文件
            _abManifest = _mainAssetBundleInfo.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
            if (_abManifest == null)
            {
                return false;
            }

            //构建全部AB包信息
            string[] abNames = _abManifest.GetAllAssetBundles();
            for (int i = 0; i < abNames.Length; i++)
            {
                if (abNames[i] == E_AssetBundleType.Scene.ToString())
                {
                    //场景包构建场景包加载器
                    _bundleLoaderDic.Add(abNames[i], new SceneBundleLoader(abNames[i], PathManager.GetAbLoadPath(abNames[i] + AbSuffix)));
                }
                else
                {
                    //非场景包构建资源包加载器
                    _bundleLoaderDic.Add(abNames[i], new AssetBundleLoader(abNames[i], PathManager.GetAbLoadPath(abNames[i] + AbSuffix)));
                }
            }

            return true;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetBundleType">资源所在AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="assetCallBack">资源加载结束回调</param>
        public void LoadAssetAsync<T>(E_AssetBundleType assetBundleType, string resName, UnityAction<T> assetCallBack) where T : Object
        {
            StartCoroutine(LoadAssetAsync_Cor());

            IEnumerator LoadAssetAsync_Cor()
            {
                string abName = assetBundleType.ToString().ToLower();
                if (!_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
                {
                    LogMgr.LogError($"AB包：{abName}不存在");
                }

                AssetBundleLoader assetBundleLoader = bundleLoader as AssetBundleLoader;
                //存在资源缓存，直接返回
                if (assetBundleLoader.ContainAsset(resName))
                {
                    assetCallBack?.Invoke(assetBundleLoader.GetAsset(resName) as T);
                    yield break;
                }

                //加载依赖和目标AB包
                yield return LoadDependenciesAndTargetAsync(abName, bundleLoader);
                //异步加载资源
                yield return assetBundleLoader.LoadAssetAsync<T>(resName, assetCallBack);
            }
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="assetCallBack"></param>
        public void LoadAssetAsync(E_AssetBundleType assetBundleType, string assetName, System.Type type, UnityAction<Object> assetCallBack)
        {
            StartCoroutine(LoadAssetAsync_Cor());

            IEnumerator LoadAssetAsync_Cor()
            {
                string abName = assetBundleType.ToString().ToLower();
                if (!_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
                {
                    LogMgr.LogError($"AB包：{abName}不存在");
                }

                AssetBundleLoader assetBundleLoader = bundleLoader as AssetBundleLoader;
                //存在资源缓存，直接返回
                if (assetBundleLoader.ContainAsset(assetName))
                {
                    assetCallBack?.Invoke(assetBundleLoader.GetAsset(assetName));
                    yield break;
                }

                //加载依赖和目标AB包
                yield return LoadDependenciesAndTargetAsync(abName, bundleLoader);
                //异步加载资源
                yield return assetBundleLoader.LoadAssetAsync(assetName, type, assetCallBack);
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetBundleType">资源所在AB包名</param>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        public T LoadAsset<T>(E_AssetBundleType assetBundleType, string resName) where T : Object
        {
            string abName = assetBundleType.ToString().ToLower();
            if (!_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
            {
                LogMgr.LogError($"AB包：{abName}不存在");
                return null;
            }

            AssetBundleLoader assetBundleLoader = bundleLoader as AssetBundleLoader;
            switch (bundleLoader.LoadPhase)
            {
                case E_BunldeLoadPhase.Start:
                    return LoadDependenciesAndTarget(abName, bundleLoader) ? assetBundleLoader.LoadAsset<T>(resName) : null;
                case E_BunldeLoadPhase.Loading:
                    LogMgr.LogError("正在异步加载AB包，无法同步加载");
                    return null;
                case E_BunldeLoadPhase.Finish:
                    //存在资源缓存，直接返回
                    if (assetBundleLoader.ContainAsset(resName))
                    {
                        return assetBundleLoader.GetAsset(resName) as T;
                    }
                    return assetBundleLoader.LoadAsset<T>(resName);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="assetBundleType">资源所在AB包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型Type</param>
        public Object LoadAsset(E_AssetBundleType assetBundleType, string resName, System.Type type)
        {
            string abName = assetBundleType.ToString().ToLower();
            if (!_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
            {
                LogMgr.LogError($"AB包：{abName}不存在");
                return null;
            }

            AssetBundleLoader assetBundleLoader = bundleLoader as AssetBundleLoader;
            switch (bundleLoader.LoadPhase)
            {
                case E_BunldeLoadPhase.Start:
                    return LoadDependenciesAndTarget(abName, bundleLoader) ? assetBundleLoader.LoadAsset(resName, type) : null;
                case E_BunldeLoadPhase.Loading:
                    LogMgr.LogError("正在异步加载AB包，无法同步加载");
                    return null;
                case E_BunldeLoadPhase.Finish:
                    //存在资源缓存，直接返回
                    if (assetBundleLoader.ContainAsset(resName))
                    {
                        return assetBundleLoader.GetAsset(resName);
                    }
                    return assetBundleLoader.LoadAsset(resName, type);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetBundleType">资源所在AB包名</param>
        /// <param name="assetsCallBack">资源加载结束回调</param>
        public void LoadAllAssetAsync<T>(E_AssetBundleType assetBundleType, UnityAction<T[]> assetsCallBack) where T : Object
        {
            StartCoroutine(LoadAllAssetAsync_Cor());

            IEnumerator LoadAllAssetAsync_Cor()
            {
                string abName = assetBundleType.ToString().ToLower();
                if (!_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
                {
                    LogMgr.LogError($"AB包：{abName}不存在");
                }

                AssetBundleLoader assetBundleLoader = bundleLoader.ConvertTo< AssetBundleLoader>();
                //加载依赖和目标AB包
                yield return LoadDependenciesAndTargetAsync(abName, bundleLoader);
                //异步加载所有资源
                yield return assetBundleLoader.LoadAllAssetsAsync<T>(assetsCallBack);
            }
        }

        /// <summary>
        /// 异步加载场景AB包
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="onLoadProgress"></param>
        public IEnumerator LoadSceneAssetBundleAsync(E_AssetBundleType assetBundleType, UnityAction<float> onLoadProgress)
        {
            string abName = assetBundleType.ToString().ToLower();
            if (_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
            {
                yield return LoadDependenciesAndTargetAsync(abName, bundleLoader, onLoadProgress);
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetBundleType">AB包类型</param>
        /// <param name="assetName">卸载的资源名称</param>
        public void UnloadAsset(E_AssetBundleType assetBundleType, string assetName)
        {
            string abName = assetBundleType.ToString().ToLower();
            if (_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
            {
                AssetBundleLoader assetBundleLoader = bundleLoader.ConvertTo<AssetBundleLoader>();
                //卸载资源
                assetBundleLoader.UnloadAsset(assetName);
                //引用计数为0，卸载AB包
                if (assetBundleLoader.RefCount == 0)
                {
                    assetBundleLoader.Unload();
                }
            }
        }

        /// <summary>
        /// 异步卸载资源
        /// </summary>
        /// <param name="assetBundleType">AB包类型</param>
        /// <param name="assetName">卸载的资源名称</param>
        /// <param name="onUnloadAsset">卸载回调</param>
        public void UnloadAssetAsync(E_AssetBundleType assetBundleType, string assetName, UnityAction onUnloadAsset = null)
        {
            StartCoroutine(UnloadAssetAsync_Cor());

            IEnumerator UnloadAssetAsync_Cor()
            {
                string abName = assetBundleType.ToString().ToLower();
                if (_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
                {
                    AssetBundleLoader assetBundleLoader = bundleLoader.ConvertTo<AssetBundleLoader>();
                    //卸载资源
                    assetBundleLoader.UnloadAsset(assetName);
                    //引用计数为0，卸载AB包
                    if (assetBundleLoader.RefCount == 0)
                    {
                        yield return assetBundleLoader.UnloadAsync();
                    }
                }
                onUnloadAsset?.Invoke();
            }
        }

        /// <summary>
        /// 获取目标包中所有的资源名
        /// </summary>
        /// <param name="abName">目标包</param>
        /// <param name="onAssetNamesLoad">资源名回调</param>
        public void GetAllAssetNamesInAssetBundle(E_AssetBundleType assetBundleType, UnityAction<string[]> onAssetNamesLoad)
        {
            StartCoroutine(GetAllAssetNamesInAssetBundle_Cor());

            IEnumerator GetAllAssetNamesInAssetBundle_Cor()
            {
                string abName = assetBundleType.ToString().ToLower();
                if (_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
                {
                    //加载依赖和目标AB包
                    yield return LoadDependenciesAndTargetAsync(abName, bundleLoader);
                    //执行回调
                    onAssetNamesLoad?.Invoke(bundleLoader.ConvertTo<AssetBundleLoader>().GetAllAssetNames());
                }
            }
        }

        /// <summary>
        /// 获取所有的场景路径
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="onScenePathsLoad"></param>
        public void GetAllScenePaths(UnityAction<string[]> onScenePathsLoad)
        {
            StartCoroutine(GetAllAssetNamesInAssetBundle_Cor());

            IEnumerator GetAllAssetNamesInAssetBundle_Cor()
            {
                string abName = E_AssetBundleType.Scene.ToString().ToLower();
                if (_bundleLoaderDic.TryGetValue(abName, out IBundleLoader bundleLoader))
                {
                    //加载依赖和目标AB包
                    yield return LoadDependenciesAndTargetAsync(abName, bundleLoader);
                    //执行回调
                    onScenePathsLoad?.Invoke(bundleLoader.ConvertTo<SceneBundleLoader>().GetAllScenePaths());
                }
            }

        }

        /// <summary>
        /// 清空包加载管理器缓存
        /// </summary>
        public void ClearCache()
        {
            StopAllCoroutines();
            foreach (IBundleLoader bundleLoader in _bundleLoaderDic.Values)
            {
                bundleLoader.Unload();
            }
            AssetBundle.UnloadAllAssetBundles(false);
            System.GC.Collect();
        }

        /// <summary>
        /// 异步加载依赖和目标AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="bundleLoader"></param>
        /// <param name="onLoadProgress"></param>
        /// <returns></returns>
        private IEnumerator LoadDependenciesAndTargetAsync(string abName, IBundleLoader bundleLoader, UnityAction<float> onLoadProgress)
        {
            //异步加载依赖包
            string[] dependencies = _abManifest.GetAllDependencies(abName);
            //加载的AB包总数（所有依赖 + 目标）；通过AB包的加载数量均分进度
            float loadAbNum = dependencies.Length + 1;
            //每个AB包加载的临时进度
            float tempPro = 0;
            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependencie = dependencies[i];
                //开始加载
                if (_bundleLoaderDic[dependencie].LoadPhase == E_BunldeLoadPhase.Start)
                {
                    tempPro = 0;
                    yield return _bundleLoaderDic[dependencie].LoadBundleAsync((pro) => tempPro = pro / loadAbNum);
                    onLoadProgress?.Invoke(tempPro);
                }
                else if (_bundleLoaderDic[dependencie].LoadPhase == E_BunldeLoadPhase.Finish)
                {
                    //已加载完成返回1/n
                    onLoadProgress?.Invoke(1 / loadAbNum);
                }
                else
                {
                    //加载中等待
                    yield return new WaitUntil(() => _bundleLoaderDic[abName].LoadPhase == E_BunldeLoadPhase.Finish);
                }
            }

            //异步加载目标包
            if (bundleLoader.LoadPhase == E_BunldeLoadPhase.Start)
            {
                tempPro = 0;
                yield return bundleLoader.LoadBundleAsync((pro) => tempPro = pro / loadAbNum);
                onLoadProgress?.Invoke(tempPro);
            }
            else if (bundleLoader.LoadPhase == E_BunldeLoadPhase.Finish)
            {
                onLoadProgress?.Invoke(1 / loadAbNum);
            }
            else
            {
                yield return new WaitUntil(() => _bundleLoaderDic[abName].LoadPhase == E_BunldeLoadPhase.Finish);
            }
        }

        /// <summary>
        /// 异步加载依赖和目标AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="bundleLoader"></param>
        /// <returns></returns>
        private IEnumerator LoadDependenciesAndTargetAsync(string abName, IBundleLoader bundleLoader)
        {
            //异步加载依赖包
            //获取该AB包的所有依赖
            string[] dependencies = _abManifest.GetAllDependencies(abName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependencie = dependencies[i];
                //加载依赖
                if (_bundleLoaderDic[dependencie].LoadPhase == E_BunldeLoadPhase.Start)
                {
                    yield return _bundleLoaderDic[dependencie].LoadBundleAsync();
                }
                //等待依赖加载完成
                else
                {
                    yield return new WaitUntil(() => _bundleLoaderDic[abName].LoadPhase == E_BunldeLoadPhase.Finish);
                }
            }

            //异步加载目标包
            //加载目标AB包
            if (bundleLoader.LoadPhase == E_BunldeLoadPhase.Start)
            {
                yield return bundleLoader.LoadBundleAsync();
            }
            //等待目标表AB包加载完成
            else
            {
                yield return new WaitUntil(() => _bundleLoaderDic[abName].LoadPhase == E_BunldeLoadPhase.Finish);
            }
        }

        /// <summary>
        /// 同步加载依赖和目标AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="bundleLoader"></param>
        /// <returns></returns>
        private bool LoadDependenciesAndTarget(string abName, IBundleLoader bundleLoader)
        {
            //同步加载依赖包
            string[] dependencies = _abManifest.GetAllDependencies(abName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependencie = dependencies[i];
                if (_bundleLoaderDic[dependencie].LoadPhase == E_BunldeLoadPhase.Start)
                {
                    //依赖加载失败
                    if (!_bundleLoaderDic[dependencie].LoadBundle())
                    {
                        return false;
                    }
                }
            }

            //同步加载目标AB包
            return bundleLoader.LoadBundle();
        }
    }
}
