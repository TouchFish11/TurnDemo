using System.Collections;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Normal
{
    public class WizardNormalSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            yield return animationComponent.PlayToTarget(AnimNames[0]);
            // 创建普攻特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WizardNormalSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
