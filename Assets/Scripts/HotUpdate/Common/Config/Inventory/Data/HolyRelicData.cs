using System.Collections.Generic;

namespace HotUpdate.Common.Config.Inventory.Data
{
    public enum EHolyRelicType : byte
    {
        Type1,
        type2,
        Type3,
    }

    public enum EHolyRelicEntryType : byte
    {
        
    }
    
    /// <summary>
    /// 圣遗物数据，基础物品数据
    /// </summary>
    public class HolyRelicData : ItemData
    {
        public class HolyRelicEntryData
        {
            
        }

        public int level;
        
        private Dictionary<EHolyRelicEntryType, HolyRelicEntryData> entries = new();
    }
}
