using System;
using System.Collections.Generic;
using HotUpdate.Common.Config.Inventory.Data;
using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Inventory
{
    /// <summary>
    /// 物品数据集合，保存所有玩家物品数据
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ItemDataCollection
    {
        /// 下一个物品持久化ID
        [JsonProperty] public long nextPersistentId;
        /// 玩家物品列表
        [JsonProperty] public List<ItemData> items = new();
    }
}
