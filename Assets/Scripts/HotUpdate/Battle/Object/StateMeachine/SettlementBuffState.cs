using System.Collections;
using Core.Service;
using Core.UI;
using Core.Utility;
using HotUpdate.Battle.Status;
using HotUpdate.Battle.UI.Base;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Layer;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Camera;
using UnityEngine;

namespace HotUpdate.Battle.Object.StateMeachine
{
    /// <summary>
    /// 结算Buff状态
    /// </summary>
    public class SettlementBuffState : TurnState
    {
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
                ServiceLocator.Get<IUIManager>().GetController<BattleController>().MonsterStateUIManager.InActiveMonsterUIs();
                // 调整相机角度
                var rolePos = PlayerObject.GameObject.transform.position;
                rolePos = new Vector3(rolePos.x, 1, rolePos.z);
                var pos = rolePos + PlayerObject.GameObject.transform.forward * 4;
                var rotation = Quaternion.LookRotation(rolePos - pos);
            
                // 获取遮罩
                var mask = LayerGeter.GetPreBitLayer() | (1 << PlayerObject.GameObject.layer);
                // 创建相机
                yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>().CreateCamera(null, pos, rotation, mask));
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
        }

        public override void Exit()
        {

        }
    }
}
