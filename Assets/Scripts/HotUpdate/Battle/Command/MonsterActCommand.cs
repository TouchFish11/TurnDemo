using System.Collections;
using Core.Service;
using Core.UI;
using Core.Utility;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Layer;
using HotUpdate.Battle.Status;
using HotUpdate.Battle.Toughness;
using HotUpdate.Battle.UI.Base;
using HotUpdate.Camera;
using UnityEngine;

namespace HotUpdate.Battle.Command
{
    /// <summary>
    /// 怪物行动指令
    /// 包括韧性恢复和技能执行
    /// </summary>
    public class MonsterActCommand : Command, IMonsterActCommand
    {
        // 韧性恢复速度
        private const float recoverySpeed = 55f;
        // 韧性组件
        public IToughnessComponent ToughnessComponent { get; private set; }
        // 技能指令
        private ISkillCommand _skillCommand;
        
        public override int Priority { get; protected set; }

        public void Init(IToughnessComponent toughnessComponent, ISkillCommand skillCommand)
        {
            Sender = toughnessComponent.BattleEntity;
            ToughnessComponent = toughnessComponent;
            _skillCommand = skillCommand;
        }

        public override IEnumerator Execute(IBattleContext context)
        {
            // buff结算
            yield return UpdateStatus();
            if (!Sender.CanAct)
            {
                yield break;
            }
            
            // 韧性恢复
            yield return RestoreToughness();
            // 技能执行
            yield return _skillCommand.Execute(context);
        }

        /// <summary>
        /// 状态更新协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateStatus()
        {
            var statusComponent = Sender.GetComponent<StatusComponent>();
            var hasDot = StatusUtility.ContainDot(statusComponent.GetStatuses());
            if (hasDot)
            {
                // 隐藏其他怪物血量UI显示
                ServiceLocator.Get<IUIManager>().GetController<BattleController>().MonsterStateUIManager.ActiveMonsterUI(Sender);
                // 调整相机角度
                var monsterPos = Sender.GameObject.transform.position;
                monsterPos = new Vector3(monsterPos.x, 1, monsterPos.z);
                var pos = monsterPos + Sender.GameObject.transform.forward * 4;
                var rotation = Quaternion.LookRotation(monsterPos - pos);
            
                // 获取遮罩
                var preMask = LayerGeter.GetPreBitLayer();
                var mask = preMask | (1 << Sender.GameObject.layer);
                // 创建相机
                yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>().CreateCamera(null, pos, rotation, mask));
                // 调用组件方法
                Sender.GetComponent<StatusComponent>().UpdateStatus();
                // 等待Dot显示完成
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                // 调用组件方法
                Sender.GetComponent<StatusComponent>().UpdateStatus();
            }
        }
        
        /// <summary>
        /// 韧性恢复协程
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator RestoreToughness()
        {
            // 获取当前怪物的韧性组件
            var toughnessComponent = Sender.GetComponent<IToughnessComponent>();
            // 若韧性未被击破，切换为操作状态
            if (!toughnessComponent.IsToughnessBroken())
            {
                yield break;
            }
            
            // 隐藏其他怪物血量UI显示
            ServiceLocator.Get<IUIManager>().GetController<BattleController>().MonsterStateUIManager.ActiveMonsterUI(Sender);
            // 计算相机世界坐标的位置和看向
            var monsterPos = Sender.GameObject.transform.position;
            monsterPos = new Vector3(monsterPos.x, 1, monsterPos.z);
            var pos = monsterPos + Sender.GameObject.transform.forward * 4;
            var rotation = Quaternion.LookRotation(monsterPos - pos);
            
            // 获取遮罩
            var preMask = LayerGeter.GetPreBitLayer();
            var mask = preMask | (1 << Sender.GameObject.layer);
            // 创建相机
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>().CreateCamera(null, pos, rotation, mask));
            
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
            yield return _skillCommand.SkillData.SkillCastPostHandler.Handle(_skillCommand.SkillData.Skill);
        }

        public override void ResetData()
        {
            base.ResetData();
            ToughnessComponent = null;
        }
    }
}
