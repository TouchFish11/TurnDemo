using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 组件工厂
    /// </summary>
    public class ComponentFactory : IFactory
    {
        // 组件名称到组件类型的映射
        private static readonly Dictionary<string, Type> _nameToComponentTypeMap = new Dictionary<string, Type>();

        void IFactory.InitFactory()
        {
            FactoryUtility.ScanComponents(_nameToComponentTypeMap);
        }

        T IFactory.GetTypeInstance<T>()
        {
            return null;
        }

        /// <summary>
        /// 批量添加组件
        /// </summary>
        /// <param name="entityObject"></param>
        /// <param name="componentIds"></param>
        /// <returns></returns>
        public static IDictionary<Type, Component> AddComponents(IEntityObject entityObject, IEnumerable<string> componentIds)
        {
            IDictionary<Type, Component> components = new Dictionary<Type, Component>();

            foreach (string name in componentIds)
            {
                (Type type, Component component) = AddComponent(name, entityObject);
                components.Add(type, component);
            }
            return components;
        }

        /// <summary>
        /// 添加单个组件
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="entityObject"></param>
        public static (Type, Component) AddComponent(string componentName, IEntityObject entityObject)
        {
            if (_nameToComponentTypeMap.TryGetValue(componentName, out Type componentType))
            {
                Component component = entityObject.GameObject.AddComponent(componentType);
                if (component is IComponent iComponent)
                {
                    iComponent.Init(entityObject);
                }
                return (componentType, component);
            }

            LogManager.LogError($"未注册的组件：{componentName}");
            return (null, null);
        }

        /// <summary>
        /// 添加单个组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public static T AddComponent<T>(IEntityObject entityObject) where T : Component
        {
            if (_nameToComponentTypeMap.TryGetValue(typeof(T).Name, out Type type))
            {
                Component component = entityObject.GameObject.AddComponent(type);
                if (component is IComponent ic)
                {
                    ic.Init(entityObject);
                }
                return component as T;
            }

            LogManager.LogError($"{nameof(AddComponent)}未找到[{typeof(T).Name}]类型的组件");
            return null;
        }
    }
}
