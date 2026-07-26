using Newtonsoft.Json;

namespace Core.AssetBundles.Management
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SpriteAssetEntry : AssetEntry
    {
        /// 图集资源名称，该图片所在的图集，图集的资源路径，用于加载图集资源
        [JsonProperty] public string atlasAssetPath;
        /// 图集资源key，就是图集名
        [JsonProperty] public string atlasKey;

        /// <summary>
        /// 图片资源条目构造函数
        /// </summary>
        /// <param name="key">图片资源名</param>
        /// <param name="bundleName">图片所在的图集包名，即图集包的名称</param>
        /// <param name="assetName">图片资源本身的资源路径，不会去用这个路径加载图片，除非不打图集</param>
        /// <param name="assetType">资源的类型，是Texture</param>
        /// <param name="atlasAssetPath">图集资源名称，该图片所在的图集，图集的资源路径，用于加载图集资源</param>
        /// <param name="atlasKey">图集资源key</param>
        public SpriteAssetEntry(string key, string bundleName, string assetName, EAssetType assetType
        , string atlasAssetPath, string atlasKey) : base(key, bundleName, assetName, assetType)
        {
            this.atlasAssetPath = atlasAssetPath;
            this.atlasKey = atlasKey;
        }
    }
}
