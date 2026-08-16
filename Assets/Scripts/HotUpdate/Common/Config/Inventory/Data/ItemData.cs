using System;
using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Inventory.Data
{
    /// <summary>
    /// 物品数据
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ItemData
    {
        /// 物品ID
        [JsonProperty] public int itemId;
        /// 物品实例ID，可堆叠物品默认null；不可堆叠物品通过ID池获取唯一ID
        [JsonProperty] public long? persistentId;
        /// 物品数量
        [JsonProperty] public int itemNum;
        /// 是否是新获取
        [JsonProperty] public bool isNew;
    }
}
