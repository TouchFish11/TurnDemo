using System.Collections.Generic;

namespace HotUpdate.Game.Battle.StatSystem
{
    /// <summary>
    /// 实体状态集合
    /// </summary>
    public class StatSet
    {
        /// <summary>
        /// 计算属性集合
        /// </summary>
        public Dictionary<EStatType, Stat> Stats { get; } = new();
        
        /// <summary>
        /// 当前生命值
        /// </summary>
        public float CurrentHp { get; set; }
        
        /// <summary>
        /// 当前护盾值
        /// </summary>
        public float CurrentShield { get; set; }
    }
}
