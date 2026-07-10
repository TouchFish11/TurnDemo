using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Factory;
using HotUpdate.Game.Battle.Skill.Conditions;

namespace HotUpdate.Game.Battle.Skill.Factory
{
    /// <summary>
    /// 释放技能条件工厂
    /// </summary>
    public class CastSkillConditionFactory : Factory<ICastSkillCondition>, ICastSkillConditionFactory
    {
        private CastSkillConditionFactory(IHotUpdateManager hotUpdateManager) : base(hotUpdateManager)
        {
           
        }
        
        public ICastSkillCondition GetCastSkillCondition<TCondition>()where TCondition : class, ICastSkillCondition
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TCondition), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            Logger.LogError(TODO, $"未找到释放技能条件，{typeof(TCondition)}");
            return null;
        }
    }
}
