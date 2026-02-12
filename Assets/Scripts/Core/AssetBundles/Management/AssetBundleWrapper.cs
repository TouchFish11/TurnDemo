namespace Core.AssetBundles.Management
{
    // /// <summary>
    // /// AssetBundle包装器
    // /// ――封装AssetBundle相关操作，提供Task异步加载支持
    // /// </summary>
    // public class AssetBundleWrapper : BundleWrapper
    // {
    //     // AssetBundle中的已加载的资源缓存
    //     //private readonly Dictionary<string, AssetInfo> _nameToAssetInfoMap = new();
    //     
    //     public AssetBundleWrapper(string abName, string path) : base(abName, path)
    //     {
    //         
    //     }
    //
    //     /// <summary>
    //     /// 异步加载资源
    //     /// </summary>
    //     /// <typeparam name="T"></typeparam>
    //     /// <param name="assetName"></param>
    //     /// <param name="token"></param>
    //     /// <returns></returns>
    //     public async Task<T> LoadAssetAsync<T>(string assetName, CancellationToken token = default) where T : Object
    //     {
    //         // 异步加载资源
    //         var asset = await AssetBundle.LoadAssetAsync<T>(assetName).AsTask<T>(token);
    //         LogManager.Log($"{BundelName}包被引用，引用数为：{++refCount}；");
    //         return asset;
    //     }
    //
    //     /// <summary>
    //     /// 异步加载资源
    //     /// </summary>
    //     /// <param name="assetName"></param>
    //     /// <param name="type"></param>
    //     /// <param name="token"></param>
    //     /// <returns></returns>
    //     public async Task<Object> LoadAssetAsync(string assetName, Type type, CancellationToken token = default)
    //     {
    //         // 缓存中没有则异步加载
    //         var asset = await AssetBundle.LoadAssetAsync(assetName, type).AsTask<Object>(token);
    //         LogManager.Log($"{BundelName}包被引用，引用数为：{++refCount}；");
    //         return asset;
    //     }
    //
    //     /// <summary>
    //     /// 异步加载指定类型的所有资源
    //     /// </summary>
    //     /// <param name="token"></param>
    //     /// <typeparam name="T"></typeparam>
    //     /// <returns></returns>
    //     public async Task<T[]> LoadAllAssetsAsync<T>(CancellationToken token = default) where T : Object
    //     {
    //         var allAssets = new List<T>();
    //         await AssetBundle.LoadAllAssetsAsync<T>().AsTask(allAssets, token);
    //
    //         return allAssets.ToArray();
    //         
    //         var assets = new List<T>();
    //         var length = allAssets.Count;
    //         for (var i = 0; i < length; i++)
    //         {
    //             Object asset = allAssets[i];
    //             // 使用缓存的资源
    //             if (_nameToAssetInfoMap.TryGetValue(asset.name, out var assetInfo))
    //             {
    //                 assets.Add(assetInfo.GetAsset() as T);
    //             }
    //             // 缓存已加载的资源
    //             else
    //             {
    //                 assets.Add((T)asset);
    //                 assetInfo = new AssetInfo(BundelName, asset);
    //                 _nameToAssetInfoMap.Add(asset.name, assetInfo);
    //             }
    //             LogManager.Log($"{assetInfo.AssetName}资源被引用，{BundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
    //         }
    //         
    //         return assets.ToArray();
    //         
    //         // var source = TaskSourceBuilder.CreateTCS<T[]>();
    //         // abr.completed += (_) =>
    //         // {
    //         //
    //         //     List<T> assets = new List<T>(length);
    //         //     // 遍历已加载的资源
    //         //     for (int i = 0; i < length; i++)
    //         //     {
    //         //         Object asset = abr.allAssets[i];
    //         //         // 使用缓存的资源
    //         //         if (_nameToAssetInfoMap.TryGetValue(asset.name, out var assetInfo))
    //         //         {
    //         //             assets.Add(assetInfo.GetAsset() as T);
    //         //         }
    //         //         // 缓存已加载的资源
    //         //         else
    //         //         {
    //         //             assets.Add(asset as T);
    //         //             assetInfo = new AssetInfo(asset.name, asset);
    //         //             _nameToAssetInfoMap.Add(asset.name, assetInfo);
    //         //         }
    //         //         //LogManager.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
    //         //     }
    //         //     source.SetResult(assets.ToArray());
    //         // };
    //         // // 返回加载任务
    //         // return source.Task;
    //     }
    //
    //     /// <summary>
    //     /// 异步加载所有资源
    //     /// </summary>
    //     /// <param name="type"></param>
    //     /// <param name="token"></param>
    //     /// <returns></returns>
    //     public async Task<Object[]> LoadAllAssetsAsync(Type type, CancellationToken token = default)
    //     {
    //         // 存在所有缓存，直接返回
    //         if (_nameToAssetInfoMap.Count == AssetBundle.GetAllAssetNames().Length)
    //         {
    //             return _nameToAssetInfoMap.Values.ToArray(info => info.GetAsset());
    //         }
    //         
    //         // 异步加载所有资源
    //         var allAssets = new List<Object>();
    //         await AssetBundle.LoadAllAssetsAsync(type).AsTask(allAssets, token);
    //         
    //         // 要返回的资源
    //         var assets = new List<Object>();
    //         var length = allAssets.Count;
    //         // 已缓存的直接使用，未缓存的缓存
    //         for (var i = 0; i < length; i++)
    //         {
    //             var asset = allAssets[i];
    //             // 使用缓存的资源
    //             if (_nameToAssetInfoMap.TryGetValue(asset.name, out var assetInfo))
    //             {
    //                 assets.Add(assetInfo.GetAsset());
    //             }
    //             // 缓存已加载的资源
    //             else
    //             {
    //                 assets.Add(asset);
    //                 assetInfo = new AssetInfo(BundelName, asset);
    //                 _nameToAssetInfoMap.Add(asset.name, assetInfo);
    //             }
    //             LogManager.Log($"{assetInfo.AssetName}资源被引用，{BundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
    //         }
    //         
    //         return assets.ToArray();
    //         
    //         // AssetBundleRequest abr = assetBundle.LoadAllAssetsAsync(type);
    //         // TaskCompletionSource<Object[]> source = TaskSourceBuilder.CreateTCS<Object[]>();
    //         // abr.completed += (_) =>
    //         // {
    //         //     int length = abr.allAssets.Length;
    //         //     List<Object> assets = new List<Object>(length);
    //         //     // 遍历已加载的资源
    //         //     for (int i = 0; i < length; i++)
    //         //     {
    //         //         Object asset = abr.allAssets[i];
    //         //         // 使用缓存的资源
    //         //         if (_nameToAssetInfoMap.TryGetValue(asset.name, out var assetInfo))
    //         //         {
    //         //             assets.Add(assetInfo.GetAsset());
    //         //         }
    //         //         // 缓存已加载的资源
    //         //         else
    //         //         {
    //         //             assets.Add(asset);
    //         //             assetInfo = new AssetInfo(asset.name, asset);
    //         //             _nameToAssetInfoMap.Add(asset.name, assetInfo);
    //         //         }
    //         //         //LogManager.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
    //         //     }
    //         //     source.SetResult(assets.ToArray());
    //         // };
    //         // // 返回加载任务
    //         // return source.Task;
    //     }
    //
    //     /// <summary>
    //     /// 尝试获取已加载的资源
    //     /// </summary>
    //     /// <param name="assetName"></param>
    //     /// <param name="asset"></param>
    //     /// <returns></returns>
    //     public bool TryGetAsset(string assetName, out Object asset)
    //     {
    //         if(_nameToAssetInfoMap.TryGetValue(assetName, out var assetInfo))
    //         {
    //             asset = assetInfo.GetAsset();
    //             LogManager.Log($"{assetInfo.AssetName}资源被引用，{BundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
    //             return true;
    //         }
    //
    //         asset = null;
    //         return false;
    //     }
    //
    //     /// <summary>
    //     /// 获取所有资源
    //     /// </summary>
    //     /// <typeparam name="T"></typeparam>
    //     /// <returns></returns>
    //     public T[] GetAssets<T>() where T : Object
    //     {
    //         return _nameToAssetInfoMap.Values.ToArray(assetInfo => assetInfo.GetAsset() as T);
    //     }
    //
    //     /// <summary>
    //     /// 获取所有资源
    //     /// </summary>
    //     /// <returns></returns>
    //     public Object[] GetAssets()
    //     {
    //         return _nameToAssetInfoMap.Values.ToArray(assetInfo => assetInfo.GetAsset());
    //     }
    //
    //     /// <summary>
    //     /// 卸载指定资源
    //     /// </summary>
    //     /// <param name="assetName"></param>
    //     public void UnloadAsset(string assetName)
    //     {
    //         //存在缓存资源
    //         if (!_nameToAssetInfoMap.TryGetValue(assetName, out var assetInfo))
    //         {
    //             return;
    //         }
    //         
    //         // 卸载资源
    //         assetInfo.Unload();
    //         // 引用计数为0则从缓存中移除
    //         if (assetInfo.RefCount != 0)
    //         {
    //             return;
    //         }
    //         
    //         _nameToAssetInfoMap[assetName] = null;
    //         _nameToAssetInfoMap.Remove(assetName);
    //         LogManager.Log($"{BundelName}包引用数：{RefCount}，没有任何资源引用");
    //     }
    //
    //     /// <summary>
    //     /// 获取所有资源名称
    //     /// </summary>
    //     /// <returns></returns>
    //     public string[] GetAllAssetNames()
    //     {
    //         if (AssetBundle != null)
    //         {
    //             return AssetBundle.GetAllAssetNames();
    //         }
    //         
    //         LogManager.LogError($"获取资源名称失败，{BundelName}包未加载");
    //         return Array.Empty<string>();
    //     }
    //
    //     /// <summary>
    //     /// AssetBundle引用计数
    //     /// </summary>
    //     public override uint RefCount
    //     {
    //         get
    //         {
    //             refCount = 0;
    //             foreach (var assetInfo in _nameToAssetInfoMap)
    //             {
    //                 refCount += assetInfo.Value.RefCount;
    //             }
    //             return refCount;
    //         }
    //     }
    // }
}
