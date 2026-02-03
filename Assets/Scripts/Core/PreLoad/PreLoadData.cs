using System;
using Core.AssetBundles.Management;

namespace Core.PreLoad
{
    public readonly struct PreLoadData
    {
        public EAssetBundleType assetBundleType { get; }
            
        public Type assetType { get; }
            
        public string assetName { get; }

        public PreLoadData(EAssetBundleType assetBundleType, string assetName, Type assetType)
        {
            this.assetBundleType = assetBundleType;
            this.assetName = assetName;
            this.assetType = assetType;
        }
    }
}
