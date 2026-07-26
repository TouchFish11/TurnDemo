namespace HotUpdate.Game.InventoryModule.Sorts
{
    /// <summary>
    /// 物品排序类型
    /// </summary>
    public enum EItemSort : byte
    {
        /// <summary>
        /// 默认排序，按照物品ID排序
        /// </summary>
        Default,
        
        /// <summary>
        /// 按照物品品质排序
        /// </summary>
        Quality,
    }
}
