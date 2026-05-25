using Core.DI;
using Core.Reflection;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;

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
                    var handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    var warriorNormalSkill = DIContainer.Create<WarriorNormalSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(warriorNormalSkill, handler);
                case 11:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    var warriorBattleSkill = DIContainer.Create<WarriorBattleSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(warriorBattleSkill, handler);
                case 12:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    var warriorUltimateSkill = DIContainer.Create<WarriorUltimateSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(warriorUltimateSkill, handler);
                default:
                    return null;
            }
        }
    }
}
