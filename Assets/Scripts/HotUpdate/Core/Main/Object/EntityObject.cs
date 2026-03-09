using System.Collections.Generic;
using Core.Components;
using Core.Log;
using Core.Types;
using Core.Utility;
using HotUpdate.Core.Component;
using UnityEngine;

namespace HotUpdate.Core.Main.Object
{
    /// <summary>
    /// 实体对象抽象基类
    /// 所有游戏实体的基类，封装组件管理、生命周期（初始化/销毁）等核心逻辑
    /// 继承MonoBehaviour以挂载到GameObject，实现IEntityObject接口规范实体行为约束
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class EntityObject : MonoBehaviour, IEntityObject
    {
        /// <summary>
        /// 自定义组件缓存映射表
        /// Key：组件类型标识（TypeIdentifier），Value：对应的IComponent组件实例
        /// </summary>
        private readonly Dictionary<TypeIdentifier, IComponent> typeToIComponentMap = new();

        /// <summary>
        /// 当前实体绑定的GameObject
        /// 简化外部访问当前挂载的GameObject实例（等价于this.gameObject）
        /// </summary>
        public GameObject GameObject => gameObject;

        /// <summary>
        /// 实体属性
        /// 存储实体属性，由子类负责初始化赋值
        /// </summary>
        public EntityProperty EntityProperty { get; protected set; }
        
        /// <summary>
        /// 实体基础初始化方法
        /// 所有子类必须实现此方法，完成实体的核心初始化逻辑（如属性赋值、组件初始化等）
        /// </summary>
        /// <param name="id">实体唯一标识ID（全局唯一，用于区分不同实体）</param>
        public abstract void BaseInit(int id);

        private void OnEnable()
        {
            OnActive();
        }

        /// <summary>
        /// 在激活时调用
        /// </summary>
        protected virtual void OnActive()
        {
            
        }
        
        /// <summary>
        /// 获取当前GameObject上的指定自定义组件（重写MonoBehaviour原生方法）
        /// 适配自定义组件体系，优先从缓存获取，未命中则从GameObject查找并缓存
        /// </summary>
        /// <typeparam name="TComponent">目标组件类型，需同时继承UnityEngine.Component和IComponent</typeparam>
        /// <returns>找到的组件实例；未找到则返回null</returns>
        public new TComponent GetComponent<TComponent>() where TComponent : IComponent
        {
            // 优先从缓存获取，避免重复查找GameObject
            if (typeToIComponentMap.TryGetValue(typeof(TComponent).ToIdentifier(), out var component))
            {
                return (TComponent)component;
            }
            
            // 缓存未命中时，从当前GameObject查找组件
            if (!TryGetComponent<TComponent>(out var tComponent))
            {
                return default;
            }
            
            // 将新找到的组件存入缓存，供后续复用
            typeToIComponentMap.Add(typeof(TComponent).ToIdentifier(), tComponent);
            return tComponent;
        }

        /// <summary>
        /// 获取当前实体子物体中的指定自定义组件（重写MonoBehaviour原生方法）
        /// 适配自定义组件体系，优先从缓存获取，未命中则递归子物体查找并缓存
        /// </summary>
        /// <typeparam name="TComponent">目标组件类型，需同时继承UnityEngine.Component和IComponent</typeparam>
        /// <returns>找到的组件实例；未找到则返回null</returns>
        public new TComponent GetComponentInChildren<TComponent>() where TComponent : IComponent
        {
            // 优先从缓存获取已缓存的组件
            if (typeToIComponentMap.TryGetValue(typeof(TComponent).ToIdentifier(), out var component))
            {
                return (TComponent)component;
            }
            
            // 缓存未命中时，递归子物体查找组件
            component = base.GetComponentInChildren<TComponent>();
            if (component == null)
            {
                LogManager.LogWarning($"未从{gameObject}的子对象中找到组件：{typeof(TComponent)}");
            }
            
            // 将找到的组件存入缓存（即使为null也缓存，避免重复查找）
            typeToIComponentMap.Add(typeof(TComponent).ToIdentifier(), component);
            return (TComponent)component;
        }
        
        /// <summary>
        /// 为当前实体添加指定类型的自定义组件
        /// 通过组件工厂创建组件，自动存入缓存字典，避免重复添加
        /// </summary>
        /// <typeparam name="TComponent">要添加的组件类型，需同时继承UnityEngine.Component和IComponent</typeparam>
        /// <returns>添加成功的组件实例</returns>
        public TComponent AddComponent<TComponent>() where TComponent : UnityEngine.Component, IComponent
        {
            IComponent returnComponent = null;
            // 通过组件工厂创建并挂载组件（封装Unity原生AddComponent逻辑）
            foreach (var component in ComponentFactory.AddComponent<TComponent>(this))
            {
                if (typeof(TComponent).ToIdentifier() == component.GetType().ToIdentifier())
                {
                    returnComponent = component;
                }
                // 将组件存入缓存（TryAdd避免重复键异常）
                typeToIComponentMap.TryAdd(component.GetType().ToIdentifier(), component);
            }

            return returnComponent as TComponent;
        }

        /// <summary>
        /// 批量为当前实体添加自定义组件（按组件名称）
        /// 基于组件名称通过工厂创建组件，自动存入缓存字典
        /// </summary>
        /// <param name="componentNames">要添加的组件名称数组（需与组件工厂的命名规则匹配）</param>
        /// <returns>是否全部添加成功：true=所有组件添加完成；false=部分/全部添加失败</returns>
        public bool AddComponents(params string[] componentNames)
        {
            var count = 0;
            // 遍历添加结果，将组件存入缓存字典
            foreach (var (type, component) in ComponentFactory.AddComponents(this, componentNames))
            {
                if (typeToIComponentMap.TryAdd(type.ToIdentifier(), component))
                {
                    count++;
                }
            }
            
            // 校验添加数量：请求数量与实际添加数量一致则视为全部成功
            return count == componentNames.Length;
        }
        
        /// <summary>
        /// 在失活时调用
        /// </summary>
        protected virtual void OnInActive()
        {
            
        }
        
        /// <summary>
        /// 销毁当前实体（自定义生命周期方法）
        /// 清理所有自定义组件、释放属性引用，子类可重写扩展销毁逻辑
        /// </summary>
        public virtual void Destroy()
        {
            // 遍历并执行所有自定义组件的销毁逻辑
            foreach (var component in typeToIComponentMap.Values)
            {
                component.Destroy();
            }
            
            // 清空组件缓存，释放内存
            typeToIComponentMap.Clear();
            // 释放实体属性引用，避免内存泄漏
            EntityProperty = null;
        }
        
        /// <summary>
        /// 实体禁用回调（Unity生命周期）
        /// 内部预留方法，子类禁止重写（private修饰），用于规范实体销毁生命周期
        /// </summary>
        private void OnDisable()
        {
            OnInActive();
        }

        /// <summary>
        /// 实体销毁回调（Unity原生生命周期）
        /// 内部预留方法，子类禁止重写（private修饰），用于规范实体销毁生命周期
        /// </summary>
        private void OnDestroy()
        {
            
        }
    }
}