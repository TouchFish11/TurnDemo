#if UNITY_EDITOR
using System.Collections.Generic;
using Core.AssetBundles.Management.Editor;

namespace Core.AssetBundles.Management
{
    internal partial class AssetBundleManager : IAssetBundleDebugInfoProvider
    {
        public List<AssetBundleDebugInfo> GetAssetBundleDebugInfos()
        {
            var assetBundleDebugInfos = new List<AssetBundleDebugInfo>();
            foreach (var bundleWrapper in _nameToWrapperMap.Values)
            {
                var assetBundleDebugInfo = new AssetBundleDebugInfo
                {
                    bundleName = bundleWrapper.BundleName,
                    refCount = bundleWrapper.RefCount,
                    accessCount = bundleWrapper.AccessCount,
                    lastAccessTime = bundleWrapper.LastAccessTime,
                    bundleSize = bundleWrapper.BundleSize
                };
                
                assetBundleDebugInfos.Add(assetBundleDebugInfo);
            }
            
            return assetBundleDebugInfos;
        }
    }
}
#endif

