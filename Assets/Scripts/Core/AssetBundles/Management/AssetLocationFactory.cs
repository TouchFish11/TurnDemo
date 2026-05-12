using Core.DI;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 资源定位对象工厂
    /// </summary>
    internal class AssetLocationFactory
    {
        /// <summary>
        /// 获取组合键的资源定位对象
        /// </summary>
        /// <param name="combineKey"></param>
        /// <returns></returns>
        public static AssetLocation GetAssetLocationCombine(string combineKey)
        {
            var assetLocation = DIContainer.Create<AssetLocation>();
            assetLocation.AssetKey = combineKey;
            assetLocation.Version = 0;
            return assetLocation;
        }

        /// <summary>
        /// 获取资源定位对象
        /// </summary>
        /// <param name="entry"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AssetLocation GetAssetLocation<T>(AssetEntry entry) where T : class
        {
            string assetKey;
            string spriteKey;
            AssetLocation.ELocationType locationType;
            
            if (entry is SpriteAssetEntry spriteAssetEntry)
            {
                assetKey = spriteAssetEntry.atlasKey;
                spriteKey = spriteAssetEntry.key;
                locationType = AssetLocation.ELocationType.Sprite;
            }
            else
            {
                assetKey = entry.key;
                spriteKey = string.Empty;
                locationType = AssetLocation.ELocationType.NonSprite;
            }
            
            // 创建资源对象
            var location = DIContainer.Create<AssetLocation>();
            location.AssetKey = assetKey;
            location.SpriteKey = spriteKey;
            location.LocationType = locationType;
            location.Version = 0;
            return location;
        }
    }
}
