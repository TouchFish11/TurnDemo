using System.Collections;
using Core.Log;
using Core.Mono;
using Core.Service;
using Game.Battle.Context;
using Game.Battle.Turn;
using GameHotUpdate.Turn;
using UnityEngine;

namespace GameHotUpdate.Battle.StateMeachine
{
    /// <summary>
    /// 入场动画状态
    /// </summary>
    public class EnterAnimationState : BattleState
    {
        public EnterAnimationState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override void Enter()
        {
            Execute();
        }

        public override void Execute()
        {
            ServiceLocator.Get<IMonoAdapter>().StartCoroutine(PlayEnterAnimation());
        }
        
        private IEnumerator PlayEnterAnimation()
        {
            // 提示战斗开始
            
            
            // 播放怪物入场动画、摄像机运镜
            
            //...
            
            LogManager.Log($"回合开始，播放入场动画");

            yield return new WaitForSeconds(1f);
            
            // 处理完毕
            BattleStateMachine.ChangeState(EBattlePhase.TurnLoop);
        }

        public override void Exit()
        {
            
        }
    }
}
