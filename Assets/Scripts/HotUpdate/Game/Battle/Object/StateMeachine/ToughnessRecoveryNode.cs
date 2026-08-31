using System.Threading.Tasks;
using Core.DI;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Toughness;
using HotUpdate.Game.Battle.UI;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 韧性恢复节点
    /// </summary>
    public class ToughnessRecoveryNode : ITurnStartNode
    {
        [Inject] private IBattleCameraManager _battleCameraManager;
        [Inject] private IUIService _uiService;
        
        // 韧性恢复速度
        private const float recoverySpeed = 55f;
        
        public async Task Execute(IBattleEntityObject battleObject)
        {
            // 获取当前怪物的韧性组件
            var toughnessComponent = battleObject.GetComponent<ToughnessComponent>();
            // 若韧性未被击破，则不处理
            if (!toughnessComponent.IsToughnessBroken())
                return;
            
            // 隐藏其他怪物血量UI显示
            ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.ActiveMonsterUI(battleObject);
            // 计算相机世界坐标的位置和看向
            var monsterPos = battleObject.GameObject.transform.position;
            monsterPos = new Vector3(monsterPos.x, 1, monsterPos.z);
            var pos = monsterPos + battleObject.GameObject.transform.forward * 4;
            var rotation = Quaternion.LookRotation(monsterPos - pos);
            
            // 获取遮罩
            var preMask = LayerGeter.GetPreBitLayer();
            var mask = preMask | (1 << battleObject.GameObject.layer);
            // 创建相机
            await _battleCameraManager.CreateCamera(null, pos, rotation, mask);
            
            float currentValue = 0;
            // 等待韧性值恢复至最大值
            while (toughnessComponent.CurrentToughnessValue < toughnessComponent.MaxToughnessVaue)
            {
                currentValue += Time.deltaTime * recoverySpeed;
                toughnessComponent.SetToughnessValue((int)currentValue, toughnessComponent.MaxToughnessVaue);
                await Task.Yield();
            }
        }
    }
}
