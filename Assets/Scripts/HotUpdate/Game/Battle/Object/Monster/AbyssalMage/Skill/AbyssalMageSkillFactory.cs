using Core.DI;
using Core.Reflection;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Handler;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 深渊法师技能工厂
    /// </summary>
    public class AbyssalMageSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 103:
                    var handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new FrostfallSkill(caster, skillId), handler);
                case 104:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new AshfallSkill(caster, skillId), handler);
                case 105:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    
                    return new SkillData(new AbyssGiftSkill(caster, skillId), handler);
                case 106:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    
                    return new SkillData(new AbyssLockSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
