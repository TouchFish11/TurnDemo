using System.Collections.Generic;
using Core.DI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.StateMeachine;

namespace HotUpdate.Game.Battle.Turn
{
    /// <summary>
    /// 战斗状态机
    /// 控制战斗循环
    /// </summary>
    public class BattleStateMachine : IBattleStateMachine
    {
        // 战斗状态缓存
        private Dictionary<EBattlePhase, IBattleState> _battleStates = new();
        // 当前战斗状态
        private IBattleState _currentState;
        
        public BattleStateMachine(IBattleContext context)
        {
            _battleStates.Add(EBattlePhase.Preparation, DIContainer.Create<PreparationState>(parameterValues: new object[] { this, context }));
            _battleStates.Add(EBattlePhase.EnterAnimation, DIContainer.Create<EnterAnimationState>(parameterValues: new object[] { this, context }));
            _battleStates.Add(EBattlePhase.TurnLoop, DIContainer.Create<TurnLoopState>(parameterValues: new object[] { this, context }));
            _battleStates.Add(EBattlePhase.Over, DIContainer.Create<BattleOverState>(parameterValues: new object[] { this, context }));
        }
        
        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <returns></returns>
        public void StartBattle()
        {
            ChangeState(EBattlePhase.Preparation);
        }
        
        public void ChangeState(EBattlePhase battlePhase)
        {
            _currentState?.Exit();
            _currentState = _battleStates[battlePhase];
            _currentState.Enter();
        }

        public void Dispose()
        {
            _currentState.Exit();
            _currentState = null;
            
            foreach (var state in _battleStates.Values)
            {
                state.Dispose();
            }
            _battleStates.Clear();
            _battleStates = null;
        }
    }
}
