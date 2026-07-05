using System;
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

        // 波次结束条件缓存
        private readonly Dictionary<EWaveVictoryConditionType, IWaveOverCondition> _waveOverConditions = new();
        
        // 当前条件列表
        private readonly HashSet<IWaveOverCondition> _currentConditions = new();

        public WaveHandler(IBattleContext battleContext)
        {
            _battleContext = battleContext;
            _waveOverConditions.Add(EWaveVictoryConditionType.EliminateAllRole, new AllRoleDeadCondition());
            _waveOverConditions.Add(EWaveVictoryConditionType.EliminateAllEnemies, new AllMonsterDeadCondition());
            // ...
        }

        /// <summary>
        /// 更新当前波次结束条件，重复类型不会被重复添加
        /// </summary>
        /// <param name="conditionTypes"></param>
        /// <exception cref="ArgumentNullException">conditionTypes为null时抛出</exception>
        public void UpdateCondition(params EWaveVictoryConditionType[] conditionTypes)
        {
            if(conditionTypes == null)
                throw new ArgumentNullException(nameof(conditionTypes));
            
            _currentConditions.Clear();
            SetDefaultConditions();
            
            foreach (var waveVictoryConditionType in conditionTypes)
            {
                _currentConditions.Add(_waveOverConditions[waveVictoryConditionType]);
            }
        }

        /// <summary>
        /// 设置默认条件
        /// </summary>
        private void SetDefaultConditions()
        {
            _currentConditions.Add(_waveOverConditions[EWaveVictoryConditionType.EliminateAllRole]);
        }
        
        /// <summary>
        /// 检查是否满足结束条件
        /// </summary>
        /// <returns></returns>
        public bool CheckOver()
        {
            foreach (var waveOverCondition in _currentConditions)
            {
                if (waveOverCondition.CheckOver(_battleContext))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
