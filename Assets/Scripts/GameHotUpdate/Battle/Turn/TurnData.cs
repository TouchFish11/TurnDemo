using System.Collections.Generic;

namespace GameHotUpdate.Battle.Turn
{
    /// <summary>
    /// 回合数据
    /// </summary>
    public struct TurnData
    {
        /// <summary>
        /// 总回合数
        /// </summary>
        public int TotalTurnNumber { get; set; }
        
        /// <summary>
        /// 波次
        /// 每波次的怪物ID
        /// </summary>
        public List<List<int>> Waves { get; set; }
    }
}
