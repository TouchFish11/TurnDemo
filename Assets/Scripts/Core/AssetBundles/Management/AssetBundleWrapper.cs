using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Builder;
using Core.Log;
using Core.Tasks.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// AssetBundle包装器
    /// ――封装AssetBundle相关操作，提供Task异步加载支持
    /// </summary>
    public class AssetBundleWrapper : BundleWrapper
    {
        // AssetBundle中的已加载的资源缓存
        private readonly Dictionary<string, AssetInfo> _nameToAssetInfoMap = new();
        
        public AssetBundleWrapper(string abName, string path) : base(abName, path)
        {

        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<T> LoadAssetAsync<T>(string assetName, CancellationToken token = default) where T : Object
        {
            // 缓存中没有则异步加载
            var asset = await assetBundle.LoadAssetAsync<T>(assetName).AsTask<T>(token);
            // 创建新资源信息
            var newAssetInfo = new AssetInfo(assetName, asset);
            // 缓存资源信息
            if (!_nameToAssetInfoMap.TryAdd(assetName, newAssetInfo))
            {
                LogManager.LogError($"资源重复加载。AB包：{assetBundle.name}，资源名：{assetName}");
            }
            else
            {
                LogManager.Log($"{assetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{newAssetInfo.RefCount}");
            }

            return asset;

            // // 缓存中没有则异步加载
            // TaskCompletionSource<T> source = TaskSourceBuilder.CreateTCS<T>();
            // AssetBundleRequest abr = assetBundle.LoadAssetAsync<T>(assetName);
            // abr.completed += (_) =>
            // {
            //     AssetInfo newAssetInfo = new AssetInfo(assetName, abr.asset as T);
            //     if (!_nameToAssetInfoMap.TryAdd(assetName, newAssetInfo))
            //     {
            //         //LogManager.LogError($"资源重复加载。包名：{assetBundle.name}，资源名：{assetName}");
            //     }
            //     else
            //     {
            //         //LogManager.Log($"{assetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{newAssetInfo.RefCount}");
            //     }
            //     source.SetResult(abr.asset as T);
            // };
            // return source.Task;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Object> LoadAssetAsync(string assetName, Type type, CancellationToken token = default)
        {
            // 缓存中没有则异步加载
            var asset = await assetBundle.LoadAssetAsync(assetName, type).AsTask<Object>(token);
            // 创建新资源信息
            var newAssetInfo = new AssetInfo(assetName, asset);
            // 缓存资源信息
            if (!_nameToAssetInfoMap.TryAdd(assetName, newAssetInfo))
            {
                LogManager.LogError($"资源重复加载。AB包：{assetBundle.name}，资源名：{assetName}");
            }
            else
            {
                LogManager.Log($"{assetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{newAssetInfo.RefCount}");
            }

            return asset;
            
            // var source = TaskSourceBuilder.CreateTCS<Object>();
            // abr.completed += (_) =>
            // {
            //     var newAssetInfo = new AssetInfo(assetName, abr.asset);
            //     if (!_nameToAssetInfoMap.TryAdd(assetName, newAssetInfo))
            //     {
            //         //LogManager.LogError($"资源重复加载。包名：{assetBundle.name}，资源名：{assetName}");
            //     }
            //     else
            //     {
            //         //LogManager.Log($"{assetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{newAssetInfo.RefCount}");
            //     }
            //     source.SetResult(abr.asset);
            // };
            // return source.Task;
        }

        /// <summary>
        /// 异步加载指定类型的所有资源
        /// </summary>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T[]> LoadAllAssetsAsync<T>(CancellationToken token = default) where T : Object
        {
            var assets = await assetBundle.LoadAllAssetsAsync<T>().AsTask<T>(token);
            //int length = assets.l;
            
            //Task.WaitAll()
            
            var source = TaskSourceBuilder.CreateTCS<T[]>();
            abr.completed += (_) =>
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
                    //LogManager.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
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
        public Task<Object[]> LoadAllAssetsAsync(Type type)
        {
            AssetBundleRequest abr = assetBundle.LoadAllAssetsAsync(type);
            TaskCompletionSource<Object[]> source = TaskSourceBuilder.CreateTCS<Object[]>();
            abr.completed += (_) =>
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
                    //LogManager.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
                }
                source.SetResult(assets.ToArray());
            };
            // 返回加载任务
            return source.Task;
        }

        /// <summary>
        /// 尝试获取已加载的资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool TryGetAsset(string assetName, out Object asset)
        {
            if(_nameToAssetInfoMap.TryGetValue(assetName, out AssetInfo assetInfo))
            {
                asset = assetInfo.GetAsset();
                //LogManager.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
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
                //LogManager.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
            }
            return assets.ToArray();
        }

        /// <summary>
        /// 获取所有资源
        /// </summary>
        /// <returns></returns>
        public Object[] GetAssets()
        {
            var assets = new List<Object>();
            foreach (var assetInfo in _nameToAssetInfoMap.Values)
            {
                assets.Add(assetInfo.GetAsset());
                //LogManager.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
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
                    //LogManager.Log($"{assetInfo.AssetName}资源被卸载，{bundelName}包引用数：{RefCount}");
                }
            }
        }

        public override Task UnloadAsync(bool unloadAllLoadedObjects)
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
            if (assetBundle != null)
            {
                return assetBundle.GetAllAssetNames();
            }
            
            //LogManager.LogError($"获取资源名称失败，{bundelName}包未加载");
            return Array.Empty<string>();
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
