using System.Collections;
using Core.DI;
using Core.Tasks;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Toughness;
using HotUpdate.Game.Battle.UI;
using UnityEngine;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 怪物行动指令
    /// </summary>
    public class MonsterActionCommand : Command
    {
        [Inject] private IBattleManager _battleManager;
        [Inject] private IBattleCameraManager _battleCameraManager;
        [Inject] private IUIService _uiService;

        // 韧性恢复速度
        private const float recoverySpeed = 55f;
        // 技能指令
        private SkillCommand _skillCommand;

        public override IBattleEntityObject Sender { get; protected set; }
        public override ECommandPriority Priority { get; protected set; }
        
        public void Init(IMonsterObject monsterObject, SkillCommand skillCommand)
        {
            Sender = monsterObject;
            Priority = ECommandPriority.MonsterActionWithToughnessRecovery;
            _skillCommand = skillCommand;
        }
        
        public override IEnumerator Execute(IBattleContext context)
        {
            yield return RecoverToughness();
            yield return _skillCommand.Execute(context);
        }
        
        /// <summary>
        /// 恢复韧性
        /// </summary>
        /// <returns></returns>
        private IEnumerator RecoverToughness()
        {
            // 获取当前怪物的韧性组件
            var toughnessComponent = Sender.GetComponent<ToughnessComponent>();
            // 若韧性未被击破，则不处理
            if (!toughnessComponent.IsToughnessBroken())
            {
                yield break;
            }

            // 隐藏其他怪物血量UI显示
            ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.ActiveMonsterUI(Sender);
            // 计算相机世界坐标的位置和看向
            var monsterPos = Sender.GameObject.transform.position;
            monsterPos = new Vector3(monsterPos.x, 1, monsterPos.z);
            var pos = monsterPos + Sender.GameObject.transform.forward * 4;
            var rotation = Quaternion.LookRotation(monsterPos - pos);
            
            // 获取遮罩
            var preMask = LayerGeter.GetPreBitLayer();
            var mask = preMask | (1 << Sender.GameObject.layer);
            // 创建相机
            yield return TaskUtility.WaitForTask(_battleCameraManager.CreateCamera(null, pos, rotation, mask));
            
            float currentValue = 0;
            // 等待韧性值恢复至最大值
            while (toughnessComponent.CurrentToughnessValue < toughnessComponent.MaxToughnessVaue)
            {
                currentValue += Time.deltaTime * recoverySpeed;
                toughnessComponent.SetToughnessValue((int)currentValue, toughnessComponent.MaxToughnessVaue);
                yield return null;
            }
        }

        public override IEnumerator ExcutePostProcess(IBattleContext context)
        {
            yield return _skillCommand.Skill.SkillContext.SkillCastPostHandler.Handle(_skillCommand.Skill.SkillContext);
        }
    }
}
