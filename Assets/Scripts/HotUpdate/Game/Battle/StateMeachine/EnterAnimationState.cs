using System.Collections;
using Core.DI;
using Core.Mono;
using Core.Utility;
using HotUpdate.Base.Manager;
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
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IBattleCameraManager _battleCameraManager;
        
        public EnterAnimationState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override void Enter()
        {
            _monoAdapter.StartCoroutine(PlayEnterAnimation());
        }
        
        private IEnumerator PlayEnterAnimation()
        {
            var controller = (IBattleController)uiService.GetPanel(EUIPanelId.BattlePanel);
            // 显示战斗开始协程
            controller.BattleUiManager.ShowBattleStart();
            
            // TODO：创建入场特效
            // ...
            
            // 设置相机mask
            var mask = LayerGeter.GetPreBitLayer() | LayerGeter.GetMonsterBitLayer();
            // 调整相机视角，Task转协程
            yield return TaskUtility.WaitForTask(_battleCameraManager.CreateCamera(null, new Vector3(0, 1, -3.5f), Quaternion.identity, mask));
            // 延迟2秒
            yield return new WaitForSeconds(1f);
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
