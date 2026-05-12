namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 资源定位
    /// </summary>
    internal class AssetLocation
    {
        internal enum ELocationType
        {
            Sprite,
            NonSprite,
        }
        
        /// <summary>
        /// 物理资源键或组合资源键
        /// </summary>
        public string AssetKey { get; set; }
        
        /// <summary>
        /// 当前有效版本号
        /// </summary>
        public int Version { get; set; }
        
        /// <summary>
        /// 该图集的子图片资源key
        /// </summary>
        public string SpriteKey { get; set; }
        
        /// <summary>
        /// 定位对象类型
        /// </summary>
        public ELocationType LocationType { get; set; }
    }
}
