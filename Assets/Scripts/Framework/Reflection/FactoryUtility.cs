using Game;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 工厂工具类
    /// </summary>
    public static class FactoryUtility
    {
        /// <summary>
        /// 扫描指定类型
        /// </summary>
        public static void ScanAllType<TValue, TAttribute>(Dictionary<Type, TValue> dic) where TValue : class where TAttribute : Attribute
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
                    dic.Add(type, Activator.CreateInstance(type) as TValue);
                }
            }
        }


        /// <summary>
        /// 扫描所有指定工厂
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="dic"></param>
        public static void ScanFactorys<TValue, TAttribute>(Dictionary<Type, TValue> dic) where TValue : class, IFactory where TAttribute : Attribute
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
                    TValue factory = Activator.CreateInstance(type) as TValue;
                    factory.InitFactory();
                    dic.Add(type, factory);
                }
            }
        }

        /// <summary>
        /// 扫描所有指定组件
        /// </summary>
        public static void ScanComponents(IDictionary<string, Type> componentDic)
        {
            // 扫描当前程序集所有继承IComponent和Component的子类
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if ((type.IsSubclassOf(typeof(IComponent)) || type.IsSubclassOf(typeof(Component))) && !type.IsAbstract)
                {
                    // 获取特性
                    ComponentIdAttribute attr = type.GetCustomAttribute<ComponentIdAttribute>();
                    if (attr != null && !componentDic.ContainsKey(attr.ComponentName))
                    {
                        componentDic.Add(attr.ComponentName, type);
                    }
                }
            }
        }
    }
}
