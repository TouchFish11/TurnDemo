using System.Threading.Tasks;
using UnityEngine;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// AB包管理器接口
    /// </summary>
    public interface IAssetBundleManager
    {
        string AbSuffix { get; }

        void ClearCache();
        bool ContainPath(string scenePath);
        Task<string[]> GetAllScenePaths();
        Task Init();
        Task<T> LoadAssetAsync<T>(EAssetBundleType assetBundleType, string assetName) where T : Object;
        Task<Object> LoadAssetAsync(EAssetBundleType assetBundleType, string assetName, System.Type type);
        Task<T[]> LoadAssetsAsync<T>(EAssetBundleType assetBundleType) where T : Object;
        Task<Object[]> LoadAssetsAsync(EAssetBundleType assetBundleType, System.Type type);
        Task<bool> LoadSceneBundleAsync();
        void UnloadAsset(EAssetBundleType assetBundleType, string assetName);
        Task<bool> UnloadBundleAsync(EAssetBundleType assetBundleType, bool unloadAllLoadedObjects = false);
    }
}
