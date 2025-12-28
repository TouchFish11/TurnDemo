using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 实体对象
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class EntityObject : MonoBehaviour, IEntityObject
    {
        // 自定义组件类型映射
        private readonly Dictionary<Type, IComponent> typeToIComponentMap = new Dictionary<Type, IComponent>();
        // 内置组件类型映射
        private readonly Dictionary<Type, Component> typeToComponentMap = new Dictionary<Type, Component>();

        // 组件配置
        [SerializeField] private ComponentConfig componentConfig;

        public GameObject GameObject => this.gameObject;

        public EntityProperty EntityProperty { get; protected set; }

        protected virtual void Awake()
        {

        }

        public virtual void BaseInit(int id)
        {

        }

        public new TComponent GetComponent<TComponent>() where TComponent : Component
        {
            // 先从缓存中查找自定义组件
            if (typeToIComponentMap.TryGetValue(typeof(TComponent), out var iComponent))
            {
                return iComponent as TComponent;
            }

            // 从缓存中查找内置组件
            if (typeToComponentMap.TryGetValue(typeof(TComponent), out var component))
            {
                return component as TComponent;
            }

            // 从对象上查找内置组件
            if (base.TryGetComponent<TComponent>(out var tComponent))
            {
                // 缓存内置组件
                typeToComponentMap.Add(typeof(TComponent), tComponent);
                return tComponent;
            }

            return null;
        }

        public new TComponent GetComponentInChildren<TComponent>() where TComponent : Component
        {
            return base.GetComponentInChildren<TComponent>();
        }

        public TComponent AddComponent<TComponent>() where TComponent : Component
        {
            TComponent component = ComponentFactory.AddComponent<TComponent>(this);
            // 缓存自定义组件
            if (component is IComponent iComponent)
            {
                typeToIComponentMap.TryAdd(typeof(TComponent), iComponent);
            }
            // 缓存内置组件
            else
            {
                typeToComponentMap.TryAdd(typeof(TComponent), component);
            }
            return component;
        }

        public bool AddComponents(params int[] componentIds)
        {
            var components = ComponentFactory.AddComponents(this, componentConfig == null ? componentIds : componentConfig.compnentIds);
            foreach (var info in components)
            {
                if (info.Value is IComponent iComponent)
                {
                    typeToIComponentMap.TryAdd(info.Key, iComponent);
                }
                else
                {
                    typeToComponentMap.TryAdd(info.Key, info.Value);
                }
            }
            return components.Count == componentIds.Length;
        }
    }
}
