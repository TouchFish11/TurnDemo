using System.Collections.Generic;
using Test;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem
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
        public float CurrentShiled { get; set; }
    }
}
