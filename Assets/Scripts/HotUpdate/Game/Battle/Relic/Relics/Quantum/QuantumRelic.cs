using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Relic.Relics.Quantum
{
    /// <summary>
    /// ������װ��2���ף�������+12%��4���ף�������׷�������˺���
    /// </summary>
    public class QuantumRelic : IRelic
    {
        public int RelicID => 0;
        public string Name { get; } = "����֮Ӱ";
        public E_RelicRarity Rarity { get; } = E_RelicRarity.Legendary;
        public List<RelicEffect> SingleEffects { get; } = new()
        {
            // ����Ч����������+12%
            new RelicEffect(E_RelicBoun.CriticalRate, 12),
            // ����Ч���������˺�+14%
            new RelicEffect(E_RelicBoun.CriticalDmg, 14),
            // ����Ч����С����+24
            new RelicEffect(E_RelicBoun.BuildHp, 24),
            // ����Ч�����ٶ�+4
            new RelicEffect(E_RelicBoun.Speed, 4),
        };
    }
}
