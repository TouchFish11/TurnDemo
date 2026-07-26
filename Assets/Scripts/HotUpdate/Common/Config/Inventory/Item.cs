using Core.Pool;
using HotUpdate.Common.Config.Inventory.Config;

namespace HotUpdate.Common.Config.Inventory
{
    /// <summary>
    /// 物品对象，可通过缓存池复用
    /// </summary>
    public class Item : IPoolData
    {
        // 物品持久化ID
        public long persistentId;
        
        // 配置数据
        public ItemConfig itemConfig;
        
        // 玩家数据
        public int auxValue;    // 物品辅助数据，数量/强化等级/收藏星级等
        public bool isNew;
        
        // 物品状态
        public bool isDeleted;  // 是否删除
        
        void IPoolData.ResetData()
        {
            itemConfig = null;
        }
    }
}
