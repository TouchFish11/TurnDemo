using System.Collections;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Battle
{
    public class WizardBattleSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            yield return animationComponent.PlayToTarget(AnimNames[0]);
            
            // 触发技能特效：通过特效管理器创建战技特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WizardBattleSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
