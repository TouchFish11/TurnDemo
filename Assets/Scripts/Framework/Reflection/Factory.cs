using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework
{
    /// <summary>
    /// 工厂
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TAttribute"></typeparam>
    public abstract class Factory<TValue, TAttribute> : IFactory where TValue : class where TAttribute : Attribute
    {
        protected readonly Dictionary<Type, TValue> typeToIStatusMap = new Dictionary<Type, TValue>();

        public void InitFactory()
        {
            ScanAllIType();
        }

        public virtual T GetValue<T>() where T : class
        {
            if (typeToIStatusMap.TryGetValue(typeof(T), out var value))
            {
                return value as T;
            }

            return default;
        }

        /// <summary>
        /// 扫描指定类型
        /// </summary>
        protected virtual void ScanAllIType()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                TAttribute attribute = type.GetCustomAttribute<TAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                if (typeof(TValue).IsAssignableFrom(type))
                {
                    typeToIStatusMap.Add(type, Activator.CreateInstance(type) as TValue);
                }
            }
        }
    }
}
