using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// AssetBundle包装器
    /// ——封装AssetBundle相关操作，提供Task异步加载支持
    /// </summary>
    public class AssetBundleWrapper : BundleWrapper
    {
        // AssetBundle中的已加载的资源缓存
        private readonly Dictionary<string, AssetInfo> _nameToAssetInfoMap = new Dictionary<string, AssetInfo>();

        public AssetBundleWrapper(string abName, string path) : base(abName, path)
        {

        }

        /// <summary>
        ///  异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Task<T> LoadAssetAsync<T>(string assetName) where T : Object
        {
            // 先从缓存中查找
            if (_nameToAssetInfoMap.TryGetValue(assetName, out AssetInfo assetInfo))
            {
                LogMgr.Log($"{assetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
                return Task.FromResult(assetInfo.GetAsset() as T);
            }

            // 缓存中没有则异步加载
            AssetBundleRequest abr = assetBundle.LoadAssetAsync<T>(assetName);
            TaskCompletionSource<T> source = new TaskCompletionSource<T>();
            abr.completed += (asyncOperation) =>
            {
                AssetInfo newAssetInfo = new AssetInfo(assetName, abr.asset as T);
                _nameToAssetInfoMap.Add(assetName, newAssetInfo);
                LogMgr.Log($"{assetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{newAssetInfo.RefCount}");
                source.SetResult(abr.asset as T);
            };
            return source.Task;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<Object> LoadAssetAsync(string assetName, System.Type type)
        {
            // 先从缓存中查找
            if (_nameToAssetInfoMap.TryGetValue(assetName, out AssetInfo assetInfo))
            {
                LogMgr.Log($"{assetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
                return Task.FromResult(assetInfo.GetAsset());
            }

            // 缓存中没有则异步加载
            AssetBundleRequest abr = assetBundle.LoadAssetAsync(assetName, type);
            TaskCompletionSource<Object> source = new TaskCompletionSource<Object>();
            abr.completed += (asyncOperation) =>
            {
                AssetInfo newAssetInfo = new AssetInfo(assetName, abr.asset);
                _nameToAssetInfoMap.Add(assetName, newAssetInfo);
                source.SetResult(abr.asset);
                LogMgr.Log($"{assetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{newAssetInfo.RefCount}");
            };
            return source.Task;
        }

        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Task<T[]> LoadAllAssetsAsync<T>() where T : Object
        {
            AssetBundleRequest abr = assetBundle.LoadAllAssetsAsync<T>();
            TaskCompletionSource<T[]> source = new TaskCompletionSource<T[]>();
            abr.completed += (asyncOperation) =>
            {
                int length = abr.allAssets.Length;
                List<T> assets = new List<T>(length);
                // 遍历已加载的资源
                for (int i = 0; i < length; i++)
                {
                    Object asset = abr.allAssets[i];
                    // 使用缓存的资源
                    if (_nameToAssetInfoMap.TryGetValue(asset.name, out var assetInfo))
                    {
                        assets.Add(assetInfo.GetAsset() as T);
                    }
                    // 缓存已加载的资源
                    else
                    {
                        assets.Add(asset as T);
                        assetInfo = new AssetInfo(asset.name, asset);
                        _nameToAssetInfoMap.Add(asset.name, assetInfo);
                    }
                    LogMgr.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
                }
                source.SetResult(assets.ToArray());
            };
            // 返回加载任务
            return source.Task;
        }

        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <returns></returns>
        public Task<Object[]> LoadAllAssetsAsync(System.Type type)
        {
            AssetBundleRequest abr = assetBundle.LoadAllAssetsAsync(type);
            TaskCompletionSource<Object[]> source = new TaskCompletionSource<Object[]>();
            abr.completed += (asyncOperation) =>
            {
                int length = abr.allAssets.Length;
                List<Object> assets = new List<Object>(length);
                // 遍历已加载的资源
                for (int i = 0; i < length; i++)
                {
                    Object asset = abr.allAssets[i];
                    // 使用缓存的资源
                    if (_nameToAssetInfoMap.TryGetValue(asset.name, out var assetInfo))
                    {
                        assets.Add(assetInfo.GetAsset());
                    }
                    // 缓存已加载的资源
                    else
                    {
                        assets.Add(asset);
                        assetInfo = new AssetInfo(asset.name, asset);
                        _nameToAssetInfoMap.Add(asset.name, assetInfo);
                    }
                    LogMgr.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
                }
                source.SetResult(assets.ToArray());
            };
            // 返回加载任务
            return source.Task;
        }

        /// <summary>
        /// 尝试获取已加载的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool TryGetAsset(string assetName, out Object asset)
        {
            if(_nameToAssetInfoMap.TryGetValue(assetName, out AssetInfo assetInfo))
            {
                asset = assetInfo.GetAsset();
                LogMgr.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
                return true;
            }
            else
            {
                asset = null;
                return false;
            }
        }

        /// <summary>
        /// 获取所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetAssets<T>() where T : Object
        {
            List<T> assets = new List<T>();
            foreach (var assetInfo in _nameToAssetInfoMap.Values)
            {
                assets.Add(assetInfo.GetAsset() as T);
                LogMgr.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
            }
            return assets.ToArray();
        }

        /// <summary>
        /// 获取所有资源
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object[] GetAssets()
        {
            List<Object> assets = new List<Object>();
            foreach (var assetInfo in _nameToAssetInfoMap.Values)
            {
                assets.Add(assetInfo.GetAsset());
                LogMgr.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
            }
            return assets.ToArray();
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <param name="assetName"></param>
        public void UnloadAsset(string assetName)
        {
            //存在缓存资源
            if (_nameToAssetInfoMap.TryGetValue(assetName, out var assetInfo))
            {
                // 卸载资源
                assetInfo.Unload();
                // 引用计数为0则从缓存中移除
                if (assetInfo.RefCount == 0)
                {
                    _nameToAssetInfoMap[assetName] = null;
                    _nameToAssetInfoMap.Remove(assetName);
                    LogMgr.Log($"{assetInfo.AssetName}资源被卸载，{bundelName}包引用数：{RefCount}");
                }
            }
        }

        public override Task<bool> UnloadAsync(bool unloadAllLoadedObjects)
        {
            // 清空已加载资源缓存
            _nameToAssetInfoMap.Clear();
            // 调用基类卸载方法
            return base.UnloadAsync(unloadAllLoadedObjects);
        }

        /// <summary>
        /// 获取所有资源名称
        /// </summary>
        /// <returns></returns>
        public string[] GetAllAssetNames()
        {
            if (assetBundle == null)
            {
                LogMgr.LogError($"获取资源名称失败，{bundelName}包未加载");
                return new string[0];
            }
            return assetBundle.GetAllAssetNames();
        }

        /// <summary>
        /// AssetBundle引用计数
        /// </summary>
        public override uint RefCount
        {
            get
            {
                refCount = 0;
                foreach (var assetInfo in _nameToAssetInfoMap)
                {
                    refCount += assetInfo.Value.RefCount;
                }
                return refCount;
            }
        }
    }
}
