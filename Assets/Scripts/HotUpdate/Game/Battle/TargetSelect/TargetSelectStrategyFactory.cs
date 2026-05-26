using Core.DI;
using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Factory;

namespace HotUpdate.Game.Battle.TargetSelect
{
    /// <summary>
    /// 目标选择策略工厂
    /// </summary>
    public class TargetSelectStrategyFactory : Factory<ITargetSelectStrategy>, ITargetSelectStrategyFactory
    {
        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, DIContainer.GetInstance<IHotUpdateManager>().GetAssemblies());
        }
        
        public ITargetSelectStrategy GetTargetSelectStrategy<TStrategy>()where TStrategy : class, ITargetSelectStrategy
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TStrategy), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            Logger.LogError($"未找到目标选择策略，{typeof(TStrategy)}");
            return null;
        }
    }
}
