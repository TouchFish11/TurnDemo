using System.Collections.Generic;

namespace Core.AssetBundles.Management.Editor
{
    public interface IAssetDebugInfoProvider
    {
        List<AssetDebugInfo> GetAssetDebugInfos();
    }
    
    public struct AssetDebugInfo
    {
        public string assetName;
        public string assetPath;
        public string bundleName;
        public uint refCount;
        public long assetSize;
    }
}