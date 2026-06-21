using System.Collections;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Normal
{
    public class WizardNormalSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        // 普攻动画状态名称
        private const string AttackState = "NormalAttack";
        
        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<IBattleAnimationComponent>();
            // 设置技能对应的动画状态
            animationComponent.SetAnimationState(SkillContext.SkillInfo.f_animationType);
            // 等待动画播放到普攻状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(AttackState));
            // 创建普攻特效
            yield return TaskUtility.WaitForTask(
                vfxManager.CreateVFX(AssetKeys.VFX_WizardNormalSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo)
                , projectile => SkillContext.Projectile = projectile);
        }
    }
}
