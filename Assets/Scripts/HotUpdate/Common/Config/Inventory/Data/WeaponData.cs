using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Inventory.Data
{
    /// <summary>
    /// 武器数据
    /// </summary>
    public class WeaponData : ItemData
    {
        // 武器等级
        [JsonProperty] public int level;
    }
}
