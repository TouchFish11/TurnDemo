using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Normal
{
    public class WizardNormalSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var animName = SkillContext.SkillInfo.f_animName;
            var hash = Animator.StringToHash(animName);
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            // 设置技能对应的动画状态
            animationComponent.SetSkillState(animName);
            // 等待动画播放到普攻状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationLayer.Skill_Layer_Name).shortNameHash == hash);
            // 创建普攻特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WizardNormalSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
