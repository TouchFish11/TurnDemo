using System.Collections.Generic;
using Core.Log;
using Core.Types;
using Core.Utility;

namespace Core.Reflection
{
    /// <summary>
    /// 工厂基类
    /// </summary>
    /// <typeparam name="TIValue">接口类型</typeparam>
    public abstract class Factory<TIValue> : IFactory where TIValue : class
    {
        // 具体类型到接口的映射
        protected readonly Dictionary<TypeIdentifier, TIValue> typeToInterfaceMap = new();

        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, AssemblyUtility.GetAssemblies());
        }

        public virtual TInterface GetTypeInstance<TInterface, TInstance>() where TInterface : class where TInstance : TInterface
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TInstance).ToIdentifier(), out var instance))
            {
                return instance as TInterface;
            }
            
            LogManager.LogError($"未找到类型实例：{typeof(TInstance)}");
            return null;
        }
    }
}
