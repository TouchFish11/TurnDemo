using System.Collections;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Battle
{
    public class WarriorBattleSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 切换到技能配置的动画状态
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            yield return animationComponent.PlayToTarget(AnimNames[0]);
            // 创建战技特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WarriorBattleSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
