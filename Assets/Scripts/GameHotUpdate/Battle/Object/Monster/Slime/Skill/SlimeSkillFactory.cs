using Core.Reflection;
using Core.Service;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Factory;
using GameHotUpdate.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Interface;

namespace GameHotUpdate.Battle.Object.Monster.Slime.Skill
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
