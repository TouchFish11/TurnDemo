using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Turn
{
    /// <summary>
    /// 波次数据
    /// </summary>
    public readonly struct WaveData
    {
        /// <summary>
        /// 波次编号
        /// </summary>
        public int WaveId { get; }
            
        /// <summary>
        /// 胜利条件
        /// </summary>
        public EWaveVictoryConditionType WaveVictoryConditionType { get; }
            
        /// <summary>
        /// 本波次刷出的怪物ID列表
        /// </summary>
        public List<int> MonsterIds { get; }
            
        public WaveData(int waveId, EWaveVictoryConditionType victoryConditionType, List<int> monsterIds)
        {
            WaveId = waveId;
            WaveVictoryConditionType = victoryConditionType;
            MonsterIds = monsterIds;
        }
    }
}