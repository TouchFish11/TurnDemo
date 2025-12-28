using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 量子套装（2件套：暴击率+12%；4件套：暴击后追加量子伤害）
    /// </summary>
    public class QuantumRelic : IRelic
    {
        public int RelicID => 0;
        public string Name { get; } = "量子之影";
        public E_RelicRarity Rarity { get; } = E_RelicRarity.Legendary;
        public List<RelicEffect> SingleEffects { get; } = new()
        {
            // 词条效果：暴击率+12%
            new RelicEffect(E_RelicBoun.CriticalRate, 12),
            // 词条效果：暴击伤害+14%
            new RelicEffect(E_RelicBoun.CriticalDmg, 14),
            // 词条效果：小生命+24
            new RelicEffect(E_RelicBoun.BuildHp, 24),
            // 词条效果：速度+4
            new RelicEffect(E_RelicBoun.Speed, 4),
        };
    }
}
