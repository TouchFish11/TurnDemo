using Core.HotUpdate;
using Core.Log;
using Core.Reflection;
using Core.Service;
using Core.Utility;

namespace Game.Battle.TargetSelect
{
    /// <summary>
    /// 目标选择策略工厂
    /// </summary>
    public class TargetSelectStrategyFactory : Factory<ITargetSelectStrategy>, ITargetSelectStrategyFactory
    {
        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, ServiceLocator.Get<IHotUpdateManager>().GetAssemblies());
        }
        
        public ITargetSelectStrategy GetTargetSelectStrategy<TStrategy>()where TStrategy : class, ITargetSelectStrategy
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TStrategy).ToIdentifier(), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            LogManager.LogError($"未找到目标选择策略，{typeof(TStrategy).ToIdentifier()}");
            return null;
        }
    }
}
