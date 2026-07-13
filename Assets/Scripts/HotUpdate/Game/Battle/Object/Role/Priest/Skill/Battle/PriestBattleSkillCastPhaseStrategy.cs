using System.Collections;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Battle
{
    public class PriestBattleSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        private const float _targetEndProgress = 0.3f;

        public override IEnumerator Execute()
        {
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            yield return animationComponent.PlayToTarget(AnimNames[0], _targetEndProgress);
            // 创建普攻特效：从资源配置中获取普攻特效资源并生成
            var task = vfxManager.CreateVFX(AssetKeys.VFX_Priest_BattleSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
