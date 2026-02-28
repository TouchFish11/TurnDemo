using Core.HotUpdate;
using Core.Log;
using Core.Reflection;
using Core.Service;
using Core.Utility;
using Game.Battle.Skill.Condition;

namespace GameHotUpdate.Battle.Skill.Factory
{
    /// <summary>
    /// 释放技能条件工厂
    /// </summary>
    public class CastSkillConditionFactory : Factory<ICastSkillCondition>, ICastSkillConditionFactory
    {
        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, ServiceLocator.Get<IHotUpdateManager>().GetAssemblies());
        }
        
        public ICastSkillCondition GetCastSkillCondition<TCondition>()where TCondition : class, ICastSkillCondition
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TCondition).ToIdentifier(), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            LogManager.LogError($"未找到释放技能条件，{typeof(TCondition).ToIdentifier()}");
            return null;
        }
    }
}
