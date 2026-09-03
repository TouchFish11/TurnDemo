using System.Collections;
using System.Threading.Tasks;
using Core.DI;
using Core.Mono;
using Core.Tasks;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Battle.UI;
using UnityEngine;

namespace HotUpdate.Game.Battle.StateMeachine
{
    /// <summary>
    /// 入场动画状态
    /// </summary>
    public class EnterAnimationState : BattleState
    {
        [Inject] private IBattleManager _battleManager;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IBattleCameraManager _battleCameraManager;
        
        public EnterAnimationState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override async void Enter()
        {
            var controller = (IBattleController)uiService.GetPanel(EUIPanelId.BattlePanel);
            // 显示战斗开始协程
            controller.BattleUiManager.ShowBattleStart();
            
            // TODO：创建入场特效
            // ...
            
            // 延迟1秒
            await Task.Delay(1000);
            controller.BattleUiManager.SetActionBarActive(true);
            BattleStateMachine.ChangeState(EBattlePhase.TurnLoop);
        }

        public override void Exit()
        {

        }

        protected override void OnDispose()
        {
            _monoAdapter = null;
            _battleCameraManager = null;
        }
    }
}
