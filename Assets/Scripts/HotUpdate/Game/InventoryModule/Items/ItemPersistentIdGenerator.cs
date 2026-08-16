namespace HotUpdate.Game.InventoryModule.Items
{
    /// <summary>
    /// 物品持久化ID生成器
    /// </summary>
    public class ItemPersistentIdGenerator
    {
        // 下一个持久化ID
        private long _nextPersistentId;
        
        /// <summary>
        /// 默认不可堆叠的物品持久化ID
        /// </summary>
        public static readonly long? DefaultNotStackableId = null;
        
        public ItemPersistentIdGenerator(long initialValue)
        {
            _nextPersistentId = initialValue;
        }

        /// <summary>
        /// 分配ID
        /// </summary>
        /// <returns></returns>
        public long AllocateId()
        {
            return _nextPersistentId++;
        }

        /// <summary>
        /// 当前已分配的最大ID
        /// </summary>
        public long CurrentMaxId => _nextPersistentId - 1;
    }
}
