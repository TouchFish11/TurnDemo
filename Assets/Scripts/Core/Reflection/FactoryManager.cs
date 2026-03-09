using System.Collections.Generic;
using System.Threading.Tasks;
using Core.HotUpdate;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.Types;
using Core.Utility;

namespace Core.Reflection
{
    /// <summary>
    /// 工厂管理器
    /// 管理器所有实现IFactory的工厂
    /// </summary>
    public class FactoryManager : SingletonBase<FactoryManager> , IFactoryManager
    {
        public override int Priority => 1;
        // 工厂实例类型Type到工厂接口的映射
        private readonly Dictionary<TypeIdentifier, IFactory> typeToFactoryMap = new();

        private FactoryManager()
        {

        }

        public override Task InitAsync()
        {
            var coreAssembly = ServiceLocator.Get<IHotUpdateManager>().GetCoreModule();
            FactoryUtility.ScanAllFactory(typeToFactoryMap, coreAssembly);
            return Task.CompletedTask;
        }

        public void InitHotFactorys()
        {
            var hotAssemblies = ServiceLocator.Get<IHotUpdateManager>().GetHotAssemblies();
            FactoryUtility.ScanAllFactory(typeToFactoryMap, hotAssemblies);
        }
        
        public TISubFactory GetFactory<TISubFactory, TFactory>() where TISubFactory : class, IFactory where TFactory : TISubFactory
        {
            if (typeToFactoryMap.TryGetValue(typeof(TFactory).ToIdentifier(), out var factory))
            {
                return (TISubFactory)factory;
            }
            
            LogManager.LogError($"未找到该工厂类型,{typeof(TFactory)}");
            return null;
        }
    }
}
