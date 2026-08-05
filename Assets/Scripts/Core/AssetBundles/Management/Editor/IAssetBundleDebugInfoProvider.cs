using System.Collections.Generic;

namespace Core.AssetBundles.Management.Editor
{
    public interface IAssetBundleDebugInfoProvider
    {
        List<AssetBundleDebugInfo> GetAssetBundleDebugInfos();
    }
    
    public struct AssetBundleDebugInfo
    {
        public string bundleName;
        public uint refCount;
        public long bundleSize;
        public double lastAccessTime;
        public int accessCount;
    }
}
