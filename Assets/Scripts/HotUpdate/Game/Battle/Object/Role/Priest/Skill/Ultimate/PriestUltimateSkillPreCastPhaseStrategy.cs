using System.Collections;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Event.Skill;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Ultimate
{
    public class PriestUltimateSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 都显示立绘
            yield return battleCoordinator.ExecutePreUltimateCast(skill.SkillContext.Caster, skill.SkillContext.SkillInfo);
            
            // 终结技动画Pose
            var projectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, this);
            var projectileTrans = new ProjectileTrans(SkillContext.Caster.GameObject.transform.position, Quaternion.identity);
            var vFXInfo = poolManager.GetData<VFXInfo>();
            skill.SkillContext.Caster.GetComponent<IBattleAnimationComponent>().SetUltimatePose();
            // 终结技Pose特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_Priest_UltimatePose, projectileTrans, projectileData, vFXInfo);
            yield return TaskUtility.WaitForTask(task);
            
            // 待技能组件确认释放
            var skillComponent = skill.SkillContext.Caster.GetComponent<PlayerSkillComponent>();
            yield return new WaitUntil(() => skillComponent.IsRelease);
            // 移除Pose特效
            vfxManager.RemoveVFX(skill.SkillContext.VFXInfo);
            // 清空释放者当前能量（终结技消耗所有能量）
            skill.SkillContext.PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
            // 终结释放通用逻辑、禁用输入、更新UI显示
            var context = skill.SkillContext.Caster.Context;
            context.GetEventBus().TriggerEvent(new UltimateCastEvent(context));
            
            // 根据技能配置和选择策略，筛选出技能作用的目标
            var skillContext = skill.SkillContext;
            battleCoordinator.SetSelectSkillInfo(skillContext.SkillInfo);
            battleCoordinator.SelectTargets(skillContext.Caster, skillContext.TargetSelectStrategy);
            // TODO；暂时这样写
            battleCoordinator.InitSkillTarget(skill);
        }
    }
}
