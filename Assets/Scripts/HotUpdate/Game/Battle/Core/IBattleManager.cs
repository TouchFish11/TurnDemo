using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotUpdate.Game.Battle.Turn;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗管理器接口
    /// </summary>
    public interface IBattleManager
    {
        /// <summary>
        /// 战斗波次创建器
        /// </summary>
        WaveCreator WaveCreator { get; }
        
        /// <summary>
        /// 战斗服务对象
        /// </summary>
        BattleService BattleService { get; }

        /// <summary>
        /// 进入战斗唯一入口
        /// </summary>
        /// <param name="waveData">战斗回合数据</param>
        /// <param name="OnPreEnter">战斗进入回调</param>
        /// <param name="onBattleOver">战斗结束回调</param>
        Task EnterBattle(List<WaveData> waveData, Func<Task> OnPreEnter, Func<BattleResult, Task> onBattleOver);
    }
}
