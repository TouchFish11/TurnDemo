using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Animation;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    /// <summary>
    /// 技能释放结束阶段策略
    /// </summary>
    public abstract class SkillCastEndPhaseStrategy : SkillPhaseStrategy
    {
        protected IBattleAnimationComponent BattleAnimationComponent => SkillContext.Caster.GetComponent<IBattleAnimationComponent>();
    }
}
