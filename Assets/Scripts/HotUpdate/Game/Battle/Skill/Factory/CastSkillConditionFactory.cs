using Core.DI;
using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Factory;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Conditions;

namespace HotUpdate.Game.Battle.Skill.Factory
{
    /// <summary>
    /// 释放技能条件工厂
    /// </summary>
    public class CastSkillConditionFactory : Factory<ICastSkillCondition>, ICastSkillConditionFactory
    {
        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, DIContainer.GetInstance<IHotUpdateManager>().GetAssemblies());
        }
        
        public ICastSkillCondition GetCastSkillCondition<TCondition>()where TCondition : class, ICastSkillCondition
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TCondition), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            Logger.LogError($"未找到释放技能条件，{typeof(TCondition)}");
            return null;
        }
    }
}
