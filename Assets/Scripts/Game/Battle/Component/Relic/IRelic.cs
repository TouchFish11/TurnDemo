using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Relic
{
    /// <summary>
    /// 遗器接口（套装效果+单件效果分离）
    /// </summary>
    public interface IRelic
    {
        int RelicID {  get; }

        string Name { get; }

        /// <summary>
        /// 稀有度
        /// </summary>
        E_RelicRarity Rarity { get; }

        /// <summary>
        /// 单件效果（如攻击+10%,即词条）
        /// </summary>
        List<RelicEffect> SingleEffects { get; } 
    }
}
