using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 工厂基类
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TAttribute"></typeparam>
    public abstract class Factory<TValue, TAttribute> : IFactory where TValue : class where TAttribute : Attribute
    {
        protected readonly Dictionary<Type, TValue> typeToIStatusMap = new Dictionary<Type, TValue>();

        public void InitFactory()
        {
            FactoryUtility.ScanAllType<TValue, TAttribute>(typeToIStatusMap);
        }

        public virtual T GetValue<T>() where T : class
        {
            if (typeToIStatusMap.TryGetValue(typeof(T), out var value))
            {
                return value as T;
            }

            return default;
        }
    }
}
