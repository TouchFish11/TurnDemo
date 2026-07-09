using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotUpdate.Game.Battle.Turn;

namespace HotUpdate.Game.Battle.Core
{
    public class BattleStartupParams
    {
        /// <summary>
        /// 战斗回合数据
        /// </summary>
        public List<WaveData> WaveDatas { get; set; }
        
        /// <summary>
        /// 战斗进入回调
        /// </summary>
        public Func<Task> OnPreEnter { get; set; }
        
        /// <summary>
        /// 战斗结束回调
        /// </summary>
        public Func<BattleResult, Task> OnBattleOver { get; set; }
    }
}