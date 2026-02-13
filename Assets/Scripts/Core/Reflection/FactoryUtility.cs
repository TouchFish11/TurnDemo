using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Components;
using Core.HotUpdate;
using Core.Log;
using Core.Service;
using Core.Types;
using Core.Utility;

namespace Core.Reflection
{
    /// <summary>
    /// 工厂工具类
    /// </summary>
    public static class FactoryUtility
    {
        /// <summary>
        /// 扫描所有实现TIValue的类型
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="keyFunc"></param>
        /// <param name="valueFunc"></param>
        /// <param name="isAbstract"></param>
        /// <param name="isInterface"></param>
        /// <param name="assemblies"></param>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TIValue"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        public static void ScanAllType<TIValue, Tkey, TValue>(IDictionary<Tkey, TValue> dic, Func<Type, Tkey> keyFunc, Func<Type, TValue> valueFunc, bool isAbstract = false,
            bool isInterface = false, params Assembly[] assemblies) where TValue : class
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(TIValue).IsAssignableFrom(type) && !isAbstract && !type.IsInterface)
                    {
                        dic.Add(keyFunc.Invoke(type), valueFunc.Invoke(type));
                    }
                }
            }
        }
        
        /// <summary>
        /// 扫描所有实现TIValue的类型
        /// </summary>
        public static void ScanAllType<TIValue>(Dictionary<TypeIdentifier, TIValue> dic, params Assembly[] assemblies) where TIValue : class
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(TIValue).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    {
                        dic.Add(type.ToIdentifier(), Activator.CreateInstance(type) as TIValue);
                    }
                }
            }
        }

        /// <summary>
        /// 扫描所有实现IFactory的工厂
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        public static void ScanAllFactory<TValue>(Dictionary<TypeIdentifier, TValue> dic) where TValue : class, IFactory
        {
            foreach (var assembly in ServiceLocator.Get<IHotUpdateManager>().GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(TValue).IsAssignableFrom(type) || type.IsAbstract)
                    {
                        continue;
                    }
                    var factory = Activator.CreateInstance(type) as TValue;
                    factory?.InitFactory();
                    dic.Add(type.ToIdentifier(), factory);
                }
            }
        }

        /// <summary>
        /// 扫描所有热更组件
        /// </summary>
        public static void ScanComponents(IDictionary<string, Type> components)
        {
            // 获取热更的程序集
            foreach (var assembly in ServiceLocator.Get<IHotUpdateManager>().GetHotAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(IComponent).IsAssignableFrom(type) || type.IsAbstract)
                    {
                        continue;
                    }
                    
                    var attr = type.GetCustomAttribute<ComponentIdAttribute>();
                    if (attr != null)
                    {
                        components.TryAdd(attr.ComponentType.Name, attr.ComponentType);
                    }
                }
            }
        }
    }
}
