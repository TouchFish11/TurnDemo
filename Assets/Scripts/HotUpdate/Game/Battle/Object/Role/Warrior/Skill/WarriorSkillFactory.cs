using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.Factory;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Core;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// 战士技能工厂
    /// </summary>
    public class WarriorSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 10:
                    var handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var warriorNormalSkill = DIContainer.Create<WarriorNormalSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(warriorNormalSkill, handler);
                case 11:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var warriorBattleSkill = DIContainer.Create<WarriorBattleSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(warriorBattleSkill, handler);
                case 12:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    var warriorUltimateSkill = DIContainer.Create<WarriorUltimateSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(warriorUltimateSkill, handler);
                default:
                    return null;
            }
        }
    }
}
