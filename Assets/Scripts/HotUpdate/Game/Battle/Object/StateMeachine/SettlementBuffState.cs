using System.Collections;
using Core.DI;
using Core.Tasks;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.UI;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 结算Buff状态
    /// </summary>
    public class SettlementBuffState : TurnState
    {
        [Inject] private IUIService _uiService;
        [Inject] private IBattleCameraManager _battleCameraManager;
        
        public SettlementBuffState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            PlayerObject.StartCoroutine(UpdateState());
        }

        private IEnumerator UpdateState()
        {
            var statusComponent = PlayerObject.GetComponent<StatusComponent>();
            var hasDot = StatusUtility.ContainDot(statusComponent.GetStatuses());
            if (hasDot)
            {
                // 隐藏所有怪物血量UI显示
                (_uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController).MonsterStateUIManager.InActiveMonsterUIs();
                // 调整相机角度
                var rolePos = PlayerObject.GameObject.transform.position;
                rolePos = new Vector3(rolePos.x, 1, rolePos.z);
                var pos = rolePos + PlayerObject.GameObject.transform.forward * 4;
                var rotation = Quaternion.LookRotation(rolePos - pos);
            
                // 获取遮罩
                var mask = LayerGeter.GetPreBitLayer() | (1 << PlayerObject.GameObject.layer);
                // 创建相机
                yield return TaskUtility.WaitForTask(_battleCameraManager.CreateCamera(null, pos, rotation, mask));
                // 优化表现
                yield return new WaitForSeconds(0.2f);
                // 调用组件方法
                statusComponent.UpdateStatus();
                // 等待Dot显示完成
                yield return new WaitForSeconds(1.4f);
            }
            else
            {
                // 调用组件方法
                statusComponent.UpdateStatus();
            }
            
            // 判断能否行动
            if (PlayerObject.CanAct)
            {
                PlayerObject.ChangeState(EActPhase.TurnStart);
            }
            else
            {
                PlayerObject.ChangeState(EActPhase.TurnEnd);
            }
        }

        public override void Exit()
        {

        }
    }
}
