using System.Collections.Generic;
using HotUpdate.Game.Battle.Condition;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Turn
{
    /// <summary>
    /// 波次处理器
    /// </summary>
    public class WaveHandler
    {
        private readonly IBattleContext _battleContext;

        // 条件缓存
        private readonly Dictionary<EWaveVictoryConditionType, IWaveOverCondition> _waveOverConditions = new();
        
        // 当前条件
        private IWaveOverCondition _waveOverCondition;

        public WaveHandler(IBattleContext battleContext)
        {
            _battleContext = battleContext;
            _waveOverConditions.Add(EWaveVictoryConditionType.EliminateAllEnemies, new AllMonsterDeadCondition());
            // ...
        }

        /// <summary>
        /// 更新条件
        /// </summary>
        /// <param name="conditionType"></param>
        public void UpdateCondition(EWaveVictoryConditionType conditionType)
        {
            _waveOverCondition = _waveOverConditions[conditionType];
        }
        
        /// <summary>
        /// 检查是否满足结束条件
        /// </summary>
        /// <returns></returns>
        public bool CheckOver()
        {
            return _waveOverCondition.CheckOver(_battleContext);
        }
    }
}
