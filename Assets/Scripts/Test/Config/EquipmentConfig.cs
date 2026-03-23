using System;
using System.Collections.Generic;

namespace Test.Config
{
    /// <summary>
    /// 运行时装备配置
    /// </summary>
    [Serializable]
    public abstract class EquipmentConfig
    {
        public int id;
        public string name;
        public string description;
        public List<BonusData> bonusDatas;
    }
}
