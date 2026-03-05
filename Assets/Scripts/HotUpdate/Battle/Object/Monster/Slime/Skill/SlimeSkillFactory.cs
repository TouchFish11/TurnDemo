using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Factory;
using HotUpdate.Battle.Skill.Handler;
using HotUpdate.Battle.Skill.Interface;

namespace HotUpdate.Battle.Object.Monster.Slime.Skill
{
    /// <summary>
    /// Slime���ܹ�����
    /// </summary>
    public class SlimeSkillFactory : SkillFactory
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 101:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new SlimeSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
