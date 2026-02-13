using System.Collections;
using System.Collections.Generic;
using Core.Log;
using Core.Mono;
using Core.Service;
using Core.UI;
using Core.Utility;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Turn;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.UI.Base;
using GameHotUpdate.Battle.Utility;
using GameHotUpdate.Cameras;
using GameHotUpdate.Manager;
using GameHotUpdate.Property;
using GameHotUpdate.UI.Loading.Battle;
using GameHotUpdate.Utility;
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
            // 相机视角
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>()
                .CreateCamera(null, new Vector3(0, 1, -3.5f), Quaternion.identity));
            
            // 显示战斗开始协程
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            controller.BattleUiManager.ShowBattleStart();
            
            // 创建入场特效
            // ...
            
            // 创建怪物并缓存
            List<IBattleEntityObject> monsters = null;
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleManager>().GetTurnCreator().CreateWave(), list => monsters = list);
            foreach (var battleEntityObject in monsters)
            {
                Context.AddBattleEntity(battleEntityObject);
                Context.AddSceneMonster(battleEntityObject);
            }
            
            // 初始化行动顺序
            BattleUtility.InitOrder(Context);
            yield return new WaitForSeconds(1f);
            
            // 处理完毕
            BattleStateMachine.ChangeState(EBattlePhase.TurnLoop);
        }

        public override void Exit()
        {
            
        }
    }
}
