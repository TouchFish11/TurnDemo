using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// AssetBundle加载器
    /// </summary>
    public class AssetBundleLoader : BundleLoader
    {
        //AssetBundle中的已加载的资源缓存
        private readonly Dictionary<string, AssetLoader> _assetCache = new Dictionary<string, AssetLoader>();

        public AssetBundleLoader(string abName, string path) : base(abName, path)
        {

        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="assetCallBack"></param>
        /// <returns></returns>
        public IEnumerator LoadAssetAsync<T>(string assetName, UnityAction<T> assetCallBack) where T : Object
        {
            //键存在两种情况：正在加载或加载完成
            if (_assetCache.TryGetValue(assetName, out var info))
            {
                //正在加载
                if (!info.IsDone)
                {
                    //记录回调
                    info.AssetCallBack += (obj) => { assetCallBack?.Invoke(obj as T); };
                }
                //没有正在加载，即加载成功
                else
                {
                    //加载完成
                    assetCallBack?.Invoke(info.GetAsset() as T);
                }
                yield break;
            }

            //创建资源信息
            AssetLoader assetInfo = new AssetLoader(assetName, (obj) => assetCallBack?.Invoke(obj as T));
            //占位
            _assetCache.Add(assetName, assetInfo);
            //异步加载资源
            assetInfo.LoadAssetAsync<T>(assetBundle);
            //等待加载完成
            yield return new WaitUntil(() => assetInfo.IsDone);
            //是否卸载
            if (assetInfo.IsUnload)
            {
                _assetCache[assetName] = null;
                _assetCache.Remove(assetName);
                yield break;
            }
            //加载完成，执行回调
            assetInfo.Invoke();
            //加载失败，不存在资源，不缓存加载失败的资源
            if (!assetInfo.ContainAsset())
            {
                _assetCache[assetName] = null;
                _assetCache.Remove(assetName);
            }
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="assetCallBack"></param>
        /// <returns></returns>
        public IEnumerator LoadAssetAsync(string assetName, System.Type type, UnityAction<Object> assetCallBack)
        {
            //键存在两种情况：正在加载或加载完成
            if (_assetCache.TryGetValue(assetName, out var info))
            {
                //正在加载
                if (!info.IsDone)
                {
                    //记录回调
                    info.AssetCallBack += assetCallBack;
                }
                //没有正在加载，即加载成功
                else
                {
                    //加载完成
                    assetCallBack?.Invoke(info.GetAsset());
                }
                yield break;
            }

            //创建资源信息
            AssetLoader assetInfo = new AssetLoader(assetName, assetCallBack);
            //占位
            _assetCache.Add(assetName, assetInfo);
            //异步加载资源
            assetInfo.LoadAssetAsync(assetBundle, type);
            //等待加载完成
            yield return new WaitUntil(() => assetInfo.IsDone);
            //是否卸载
            if (assetInfo.IsUnload)
            {
                _assetCache[assetName] = null;
                _assetCache.Remove(assetName);
                yield break;
            }
            //加载完成，执行回调
            assetInfo.Invoke();
            //加载失败，不存在资源，不缓存加载失败的资源
            if (!assetInfo.ContainAsset())
            {
                _assetCache[assetName] = null;
                _assetCache.Remove(assetName);
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetName) where T : Object
        {
            T asset;
            if (_assetCache.TryGetValue(assetName, out var assetInfo))
            {
                //正在异步加载
                if (!assetInfo.ContainAsset())
                {
                    //执行同步加载
                    asset = assetInfo.LoadAsset<T>(assetBundle);
                    //要卸载或资源为空都不缓存
                    if (assetInfo.IsUnload || asset == null)
                    {
                        _assetCache[assetName] = null;
                        _assetCache.Remove(assetName);
                        return null;
                    }
                }
                else
                {
                    //返回缓存资源
                    return assetInfo.GetAsset() as T;
                }
            }

            //第一次加载，构造资源加载器
            AssetLoader info = new AssetLoader(assetName);
            //占位
            _assetCache.Add(assetName, info);
            //同步加载
            return info.LoadAsset<T>(assetBundle);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadAsset(string assetName, System.Type type)
        {
            Object asset;
            if (_assetCache.TryGetValue(assetName, out var assetInfo))
            {
                //正在异步加载
                if (!assetInfo.ContainAsset())
                {
                    //执行同步加载
                    asset = assetInfo.LoadAsset(assetBundle, type);
                    //要卸载或资源为空都不缓存
                    if (assetInfo.IsUnload || asset == null)
                    {
                        _assetCache[assetName] = null;
                        _assetCache.Remove(assetName);
                        return null;
                    }
                }
                else
                {
                    //返回缓存资源
                    return assetInfo.GetAsset();
                }
            }

            //第一次加载，构造资源加载器
            AssetLoader info = new AssetLoader(assetName);
            //占位
            _assetCache.Add(assetName, info);
            //同步加载
            return info.LoadAsset(assetBundle, type);
        }

        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetsCallBack"></param>
        /// <returns></returns>
        public IEnumerator LoadAllAssetsAsync<T>(UnityAction<T[]> assetsCallBack) where T : Object
        {
            //异步加载资源
            AssetBundleRequest request = assetBundle.LoadAllAssetsAsync<T>();
            yield return request;

            //遍历所有资源
            for (int i = 0; i < request.allAssets.Length; i++)
            {
                string assetName = request.allAssets[i].name;
                //缓存新加载的资源
                if (!_assetCache.ContainsKey(assetName))
                {
                    AssetLoader assetInfo = new AssetLoader(assetName, null);
                    //缓存新加载的资源
                    assetInfo.TrySetAsset(request.allAssets[i]);
                    _assetCache.Add(assetName, assetInfo);
                }
            }

            //获取所有缓存的资源
            List<T> assets = new List<T>();
            foreach (AssetLoader info in _assetCache.Values)
            {
                assets.Add(info.GetAsset() as T);
            }
           
            //执行回调
            assetsCallBack?.Invoke(assets.ToArray());
        }

        /// <summary>
        /// 获取所有资源名称
        /// </summary>
        /// <returns></returns>
        public string[] GetAllAssetNames()
        {
            if (assetBundle == null)
            {
                LogMgr.LogError($"获取资源名称失败，AB包：{bundelName}未加载");
                return new string[0];
            }
            return assetBundle.GetAllAssetNames();
        }

        /// <summary>
        /// 是否存在资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool ContainAsset(string assetName)
        {
            if (_assetCache.TryGetValue(assetName, out AssetLoader assetInfo))
            {
                return assetInfo.ContainAsset();
            }
            return false;
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Object GetAsset(string assetName)
        {
            return _assetCache[assetName].GetAsset();
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <param name="assetName"></param>
        public void UnloadAsset(string assetName)
        {
            //存在缓存资源
            if (_assetCache.TryGetValue(assetName, out var assetInfo))
            {
                //释放资源
                assetInfo.Release();
                //资源引用计数为0，则清除资源
                if (assetInfo.RefCount == 0)
                {
                    LogMgr.Log($"{assetInfo.Name}资源已被卸载");
                    _assetCache[assetName] = null;
                    _assetCache.Remove(assetName);
                }
            }
        }

        /// <summary>
        /// 卸载资源包
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        public override void Unload(bool unloadAllLoadedObjects = false)
        {
            _assetCache.Clear();
            base.Unload(unloadAllLoadedObjects);
        }

        /// <summary>
        /// 异步卸载资源AB
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        /// <returns></returns>
        public override IEnumerator UnloadAsync(bool unloadAllLoadedObjects = false)
        {
            _assetCache.Clear();
            return base.UnloadAsync(unloadAllLoadedObjects);
        }

        /// <summary>
        /// AB包引用数
        /// </summary>
        public override uint RefCount
        {
            get
            {
                refCount = 0;
                foreach (AssetLoader assetInfo in _assetCache.Values)
                {
                    refCount += assetInfo.RefCount;
                }
                return refCount;
            }
        }
    }
}
