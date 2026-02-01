using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Components;
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
            foreach (var assembly in AssemblyUtility.GetAssemblies())
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
                    // LogManager.Log($"{nameof(FactoryUtility)}，查找且缓存工厂：{factory}");
                }
            }
        }

        /// <summary>
        /// 扫描所有热更组件
        /// </summary>
        public static void ScanComponents(IDictionary<string, Type> components)
        {
            // 获取热更的程序集
            foreach (var assembly in AssemblyUtility.GetHotUpdateAssemblies())
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
