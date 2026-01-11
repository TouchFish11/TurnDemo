using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 工厂基类
    /// </summary>
    /// <typeparam name="TValue">接口类型</typeparam>
    public abstract class Factory<TValue> : IFactory where TValue : class
    {
        protected static readonly Dictionary<Type, TValue> typeToITypeMap = new Dictionary<Type, TValue>();

        public void InitFactory()
        {
            FactoryUtility.ScanAllType(typeToITypeMap);
        }

        public virtual T GetTypeInstance<T>() where T : class
        {
            if (typeToITypeMap.TryGetValue(typeof(T), out var value))
            {
                return value as T;
            }

            LogManager.LogError($"未找到类型实例：{typeof(T)}");
            return default;
        }
    }
}
