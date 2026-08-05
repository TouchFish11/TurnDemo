#if UNITY_EDITOR
using System.Collections.Generic;
using Core.AssetBundles.Management.Editor;

namespace Core.AssetBundles.Management
{
    internal partial class AssetManager : IAssetDebugInfoProvider
    {
        public List<AssetDebugInfo> GetAssetDebugInfos()
        {
            var catalog = GameAsset.AssetBundleManager.Catalog;
            
            var assetDebugInfos = new List<AssetDebugInfo>();
            foreach (var assetWrapper in _assetWrappers.Values)
            {
                var entry = catalog.GetEntry(assetWrapper.AssetKey);
                var assetBundleDebugInfo = new AssetDebugInfo
                {
                    assetName = entry is SpriteAssetEntry spriteAssetEntry ? spriteAssetEntry.atlasKey : entry.key,
                    assetPath = entry.assetName,
                    assetSize = entry.assetSize,
                    bundleName = assetWrapper.BundleName,
                    refCount = assetWrapper.RefCount,
                };
                
                assetDebugInfos.Add(assetBundleDebugInfo);
            }
            
            return assetDebugInfos;
        }
    }
}
#endif

