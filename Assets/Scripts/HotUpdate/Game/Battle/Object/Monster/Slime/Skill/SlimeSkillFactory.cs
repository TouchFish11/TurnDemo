using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.Factory;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Core;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Skill
{
    /// <summary>
    /// Slime技能工厂
    /// </summary>
    public class SlimeSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 101:
                    var handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var slimeSkill = DIContainer.Create<SlimeSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(slimeSkill, handler);
                default:
                    return null;
            }
        }
    }
}
