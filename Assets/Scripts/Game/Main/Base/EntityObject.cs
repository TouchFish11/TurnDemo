using Game;
using GameLogic.BattleMoudule;
using System;
using System.Collections;
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

        protected virtual void Awake()
        {
            Init(1);
        }

        public virtual void Init(int id)
        {
           // AddComponents(1);
        }

        public new TComponent GetComponent<TComponent>() where TComponent : Component
        {
            // 先从缓存中查找自定义组件
            TComponent component = typeToIComponentMap[typeof(TComponent)] as TComponent;
            if (component != null)
            {
                return component;
            }
            else
            {
                // 从缓存中查找内置组件
                component = typeToComponentMap[typeof(TComponent)] as TComponent;
                return component;
            }
        }

        public TComponent AddComponent<TComponent>() where TComponent : Component
        {
            TComponent component = ComponentFactory.Instance.AddComponent<TComponent>(this);
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
            var components = ComponentFactory.Instance.AddComponents(this, componentConfig == null ? componentIds : componentConfig.compnentIds);
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
