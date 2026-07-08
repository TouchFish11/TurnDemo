using System.Collections;
using Core.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Ultimate
{
    public class WizardUltimateSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            // 设置技能对应的动画状态
            var arr = TextUtility.Split(SkillContext.SkillInfo.f_animNames, 2);
            yield return animationComponent.PlayToTarget(arr[0]);
            // 重新初始化投射物数据（目标为主要攻击目标）
            SkillContext.ProjectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, SkillContext);
            // 更新投射物变换信息（基于主目标位置，无旋转）
            SkillContext.ProjectileTrans = new ProjectileTrans(SkillContext.MainTarget.GameObject.transform.position, Quaternion.identity);
            
            SkillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            // 创建终结技核心特效（命中目标处）
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WizardUltimateSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
