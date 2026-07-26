using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Inventory.Data
{
    /// <summary>
    /// 贵重物品数据
    /// </summary>
    public class preciousData : ItemData
    {
        // 星级
        [JsonProperty] public int starLevel;
    }
}
