using System.Collections;
using Core.DI;
using HotUpdate.Game.Battle.Event.Skill;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 等待触发终结技效果，只有pose才有此效果
    /// </summary>
    public class UltimateWaitTriggerNode : SkillNode
    {
        [Inject] private IVFXManager _vfxManager;
        
        public UltimateWaitTriggerNode(ISkill skill) : base(skill)
        {
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            var skillComponent = skill.SkillContext.Caster.GetComponent<PlayerSkillComponent>();
            // 待技能组件确认释放（阻塞直到释放条件满足）
            yield return new WaitUntil(() => skillComponent.IsRelease);
            // 移除Pose特效
            _vfxManager.RemoveVFX(skill.SkillContext.VFXInfo);
            // 清空释放者当前能量（终结技消耗所有能量）
            skill.SkillContext.PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
            // 终结释放通用逻辑、禁用输入、更新UI显示
            var context = skill.SkillContext.Caster.Context;
            context.GetEventBus().TriggerEvent(new UltimateCastEvent(context));
        }
    }
}
