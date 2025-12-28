using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AB包管理器接口
/// </summary>
public interface IAssetBundleManager
{
    string AbSuffix { get; }

    void ClearCache();
    bool ContainPath(string scenePath);
    Task<string[]> GetAllScenePaths();
    Task<bool> Init();
    Task<T> LoadAssetAsync<T>(E_AssetBundleType assetBundleType, string assetName) where T : Object;
    Task<Object> LoadAssetAsync(E_AssetBundleType assetBundleType, string assetName, System.Type type);
    Task<T[]> LoadAssetsAsync<T>(E_AssetBundleType assetBundleType) where T : Object;
    Task<Object[]> LoadAssetsAsync(E_AssetBundleType assetBundleType, System.Type type);
    Task<bool> LoadSceneBundleAsync();
    void UnloadAsset(E_AssetBundleType assetBundleType, string assetName);
    Task<bool> UnloadBundleAsync(E_AssetBundleType assetBundleType, bool unloadAllLoadedObjects = false);
}
