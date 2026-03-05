using System.Collections;
using Core.Mono;
using Core.Service;
using Core.UI;
using Core.Utility;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Layer;
using HotUpdate.Battle.Turn;
using HotUpdate.Battle.UI.Base;
using HotUpdate.Camera;
using UnityEngine;

namespace HotUpdate.Battle.StateMeachine
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
            var controller =  ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            // 显示战斗开始协程
            controller.BattleUiManager.ShowBattleStart();
            
            // 创建入场特效
            // ...
            
            // 设置相机mask
            var mask = LayerGeter.GetPreBitLayer() | LayerGeter.GetMonsterBitLayer();
            // 相机视角
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>()
                .CreateCamera(null, new Vector3(0, 1, -3.5f), Quaternion.identity, mask));
            
            yield return new WaitForSeconds(2f);
            
            // 处理完毕
            BattleStateMachine.ChangeState(EBattlePhase.TurnLoop);
        }

        public override void Exit()
        {
            
        }
    }
}
