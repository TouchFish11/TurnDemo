using System.Collections.Generic;
using Core.Log;
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
        // 工厂实例类型Type到工厂接口的映射
        private readonly Dictionary<TypeIdentifier, IFactory> typeToFactoryMap = new();

        private FactoryManager()
        {

        }

        public void InitFactorys()
        {
            FactoryUtility.ScanAllFactory(typeToFactoryMap);
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
