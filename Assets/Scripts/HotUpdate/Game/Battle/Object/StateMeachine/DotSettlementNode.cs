using System.Threading.Tasks;
using Core.DI;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.UI;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// DOT 结算
    /// </summary>
    public class DotSettlementNode : ITurnStartNode
    {
        [Inject] private IUIService _uiService;
        [Inject] private IBattleCameraManager _battleCameraManager;
        
        public async Task Execute(IBattleEntityObject battleObject)
        {
            var statusComponent = battleObject.GetComponent<StatusComponent>();
            var hasDot = StatusUtility.ContainDot(statusComponent.GetStatuses());
            if (hasDot)
            {
                // 隐藏所有怪物血量UI显示
                ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.InActiveMonsterUIs();
                // 调整相机角度
                var rolePos = battleObject.GameObject.transform.position;
                rolePos = new Vector3(rolePos.x, 1, rolePos.z);
                var pos = rolePos + battleObject.GameObject.transform.forward * 4;
                var rotation = Quaternion.LookRotation(rolePos - pos);
                var mask = LayerGeter.GetPreBitLayer() | (1 << battleObject.GameObject.layer);
                await _battleCameraManager.CreateCamera(null, pos, rotation, mask);
                // 等待Dot显示完成
                await Task.Delay(1400);
            }
        }
    }
}
