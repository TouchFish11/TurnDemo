using System;
using Newtonsoft.Json;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 资源项
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class AssetEntry
    {
        /// 用户使用的资源标识
        [JsonProperty] public string key;     
        /// 资源所在的AB包名
        [JsonProperty] public string bundleName;     
        /// 资源在AB包内的名称
        [JsonProperty] public string assetName;
        /// 资源类型
        [JsonProperty] public EAssetType assetType;
        
        public AssetEntry(string key, string bundleName, string assetName, EAssetType assetType)
        {
            this.key = key;
            this.bundleName = bundleName;
            this.assetName = assetName;
            this.assetType = assetType;
        }
    }
}
