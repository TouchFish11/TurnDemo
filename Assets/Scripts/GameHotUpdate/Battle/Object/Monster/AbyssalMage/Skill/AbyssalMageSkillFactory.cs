using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Handlers;

namespace GameHotUpdate.Battle.Object.Monster.AbyssalMage.Skill
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
                    
                    return new SkillData(new FrostfallSkill(caster, skillId, null), handler);
                case 104:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new AshfallSkill(caster, skillId, null), handler);
                default:
                    return null;
            }
        }
    }
}
