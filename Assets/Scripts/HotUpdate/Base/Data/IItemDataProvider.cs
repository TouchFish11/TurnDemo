using System.Collections.Generic;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Common.Config.Inventory.Config;
using HotUpdate.Common.Config.Inventory.Data;

namespace HotUpdate.Base.Data
{
    public interface IItemDataProvider : IDataProvider
    {
        /// <summary>
        /// 物品配置全局缓存
        /// </summary>
        Dictionary<int, ItemConfig> ConfigMap { get; }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="deltaNum">可堆叠则为初始数量；不可堆叠则为创建数量</param>
        /// <exception cref="KeyNotFoundException"></exception>
        void AddData(int itemId, int deltaNum);

        /// <summary>
        /// 移除物品数据
        /// </summary>
        /// <param name="id">可堆叠物品则为物品ID，不可堆叠物品则为实例ID</param>
        /// <param name="deltaNum">移除数量，不可堆叠的物品忽略该参数，默认移除当前实例</param>
        /// <param name="isPile">是否可堆叠</param>
        void RemoveData(long id, int deltaNum, bool isPile);

        /// <summary>
        /// 尝试获取可堆叠物品数据
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemData"></param>
        /// <returns></returns>
        bool TryGetData(int itemId, out ItemData itemData);

        /// <summary>
        /// 尝试获取不可堆叠物品数据
        /// </summary>
        /// <param name="persistentId"></param>
        /// <param name="itemData"></param>
        /// <returns></returns>
        bool TryGetInstanceData(long persistentId, out ItemData itemData);

        /// <summary>
        /// 通过物品类型获取所有的物品数据
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        IEnumerable<ItemData> GetItemsByType(EItemType itemType);
    }
}
