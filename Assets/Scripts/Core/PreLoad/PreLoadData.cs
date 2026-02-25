using System;

namespace Core.PreLoad
{
    public readonly struct PreLoadData
    {
        public string AbName { get; }
            
        public Type AssetType { get; }
            
        public string AssetName { get; }

        public PreLoadData(string abName, string assetName, Type assetType)
        {
            this.AbName = abName;
            this.AssetName = assetName;
            this.AssetType = assetType;
        }
    }
}
