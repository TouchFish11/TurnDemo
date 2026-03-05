using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Object.Monster.AbyssalMage.Skill.Handler;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Factory;
using HotUpdate.Battle.Skill.Handler;
using HotUpdate.Battle.Skill.Interface;

namespace HotUpdate.Battle.Object.Monster.AbyssalMage.Skill
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
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new FrostfallSkill(caster, skillId), handler);
                case 104:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new AshfallSkill(caster, skillId), handler);
                case 105:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    
                    return new SkillData(new AbyssGiftSkill(caster, skillId), handler);
                case 106:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    
                    return new SkillData(new AbyssLockSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
