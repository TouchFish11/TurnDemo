using System.Collections;
using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Toughness;
using UnityEngine;

namespace GameHotUpdate.Battle.Command
{
    /// <summary>
    /// 怪物行动指令
    /// 包括韧性恢复和技能执行
    /// </summary>
    public class MonsterActCommand : Game.Battle.Command.Command, IMonsterActCommand
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
            yield return RestoreToughness_Cor();

            yield return _skillCommand.Execute(context);
        }
        
        /// <summary>
        /// 韧性恢复协程
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator RestoreToughness_Cor()
        {
            // 获取当前怪物的韧性组件
            var toughnessComponent = Sender.GetComponent<IToughnessComponent>();
            // 若韧性未被击破，切换为操作状态
            if (!toughnessComponent.IsToughnessBroken())
            {
                yield break;
            }
            
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
