using System;
using System.Collections.Generic;
using System.Reflection;
using Core.DI;
using Core.HotUpdate;
using Core.Log;
using Unity.VisualScripting;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Base.ECModule
{
    /// <summary>
    /// 组件服务类
    /// 提供组件的统一创建、注册、初始化能力
    /// 核心职责：
    /// 1. 初始化时扫描并注册所有IComponent实现类到类型映射字典，避免重复反射提升性能
    /// 2. 支持按名称/泛型类型为实体对象添加组件，自动处理组件依赖并保证初始化顺序
    /// 3. 统一管理组件生命周期，确保依赖组件（RequireComponent）优先初始化
    /// 依赖说明：依赖Unity引擎的Component体系、自定义IComponent接口、日志系统、反射工具类
    /// </summary>
    public class ComponentService
    {
        // 静态映射字典：组件名称 -> 组件Type类型
        // 键：组件类名（typeof(T).Name）；值：组件的运行时Type对象
        private readonly Dictionary<string, Type> _nameToComponentTypeMap = new();
        
        // 组件初始化栈：用于处理RequireComponent依赖时的初始化顺序
        // 栈特性保证：依赖组件先初始化（栈顶先出），当前组件后初始化
        private readonly Stack<IComponent> _componentStack = new();
        
        public ComponentService(IHotUpdateManager hotUpdateManager)
        {
            ScanComponents(_nameToComponentTypeMap, hotUpdateManager);
        }

        /// <summary>
        /// 批量为实体对象添加多个组件
        /// 遍历组件名称列表，逐个调用AddComponent方法完成组件添加与初始化
        /// </summary>
        /// <param name="entityObject">实体对象容器，承载组件的GameObject归属者（IEntityObject接口实现类）</param>
        /// <param name="componentIds">要添加的组件名称列表（需与组件类名一致，且已注册到映射字典）</param>
        /// <returns>添加成功的组件类型与实例映射字典（失败的组件不会加入返回结果）</returns>
        /// <exception cref="ArgumentNullException">
        /// 1. entityObject为null时抛出
        /// 2. componentIds为null时抛出
        /// </exception>
        public IEnumerable<(Type, IComponent)> AddComponents(IEntityObject entityObject, IEnumerable<string> componentIds)
        {
            if (entityObject == null)
                throw new ArgumentNullException(nameof(entityObject), "实体对象容器不可为null");
            
            if (componentIds == null)
                throw new ArgumentNullException(nameof(componentIds), "组件名称列表不可为null");
            
            foreach (var name in componentIds)
            {
                // 遍历单个组件添加的返回结果，逐层返回
                foreach (var valueTuple in AddComponent(name, entityObject))
                {
                    yield return valueTuple;
                }
            }
        }

        /// <summary>
        /// 为实体对象添加指定名称的组件（核心基础方法）
        /// 支持通过字符串名称动态添加组件，自动处理依赖组件的初始化
        /// </summary>
        /// <param name="componentName">组件名称（需与组件类名一致，且已注册到_nameToComponentTypeMap）</param>
        /// <param name="entityObject">实体对象容器，提供组件挂载的GameObject（entityObject.GameObject不可为null）</param>
        /// <returns>
        /// 元组集合：(组件Type类型, 组件实例)
        /// 场景1：组件查找成功且初始化完成 → 返回有效元组
        /// 场景2：组件名称未注册/初始化失败 → 返回空集合
        /// </returns>
        /// <remarks>
        /// 底层调用UnityEngine.GameObject.AddComponent(Type)挂载组件，
        /// 仅处理实现IComponent接口的组件，非IComponent组件会被忽略
        /// </remarks> 
        public IEnumerable<(Type, IComponent)> AddComponent(string componentName, IEntityObject entityObject)
        {
            // 从缓存字典查找组件类型，避免重复反射
            if (_nameToComponentTypeMap.TryGetValue(componentName, out var componentType))
            {
                // 挂载Unity组件到目标GameObject
                var component = entityObject.GameObject.AddComponent(componentType);
                // 仅处理实现IComponent接口的组件，保证初始化逻辑统一
                if (component is IComponent iComponent)
                {
                    // 递归初始化当前组件及所有依赖组件，返回初始化完成的组件集合
                    foreach (var dependentComponent in RecursiveInit(entityObject, iComponent))
                    {
                        yield return (iComponent.GetType(), dependentComponent);
                    }
                }
                else
                {
                    Logger.LogWarning(ELogTags.Component, $"组件[{componentName}]未实现IComponent接口");
                }
            }
            else
            {
                // 组件名称未注册时记录错误日志，便于问题排查
                Logger.LogError(ELogTags.Component, $"{nameof(AddComponent)}<{componentName}>：未找到该类型的组件");
            }
        }
        
        /// <summary>
        /// 泛型重载：为实体对象添加指定类型的组件（类型安全版）
        /// 编译期校验组件类型合法性，避免字符串名称拼写错误
        /// </summary>
        /// <typeparam name="T">组件类型约束：必须继承UnityEngine.Component且实现IComponent接口</typeparam>
        /// <param name="entityObject">实体对象容器，提供组件挂载的GameObject（不可为null）</param>
        /// <returns>
        /// 初始化完成的IComponent组件实例集合
        /// 场景1：类型注册成功且初始化完成 → 返回有效实例
        /// 场景2：类型未注册/挂载失败 → 返回空集合
        /// </returns>
        public IEnumerable<IComponent> AddComponent<T>(IEntityObject entityObject) where T : Component, IComponent
        {
            // 通过泛型类型名称查找注册的组件Type（复用缓存字典）
            var componentTypeName = typeof(T).Name;
            if (_nameToComponentTypeMap.TryGetValue(componentTypeName, out var type))
            {
                Logger.LogDebug(ELogTags.Component, $"对象：{entityObject.GameObject.name}开始添加组件：{type}");
                // 挂载组件到目标GameObject
                var component = entityObject.GameObject.AddComponent(type);
                Logger.LogDebug(ELogTags.Component, $"对象：{entityObject.GameObject.name}添加组件：{type}" +
                                                    $"{(!component ? "失败" : "成功")}"+
                                                    $"component为null：{!component}");
                
                // 类型转换并处理初始化
                if (component is not IComponent ic)
                {
                    yield break;
                }
                
                foreach (var dependentComponent in RecursiveInit(entityObject, ic))
                {
                    yield return dependentComponent;
                }
            }
            else
            {
                // 类型未注册时记录详细错误日志
                Logger.LogError(ELogTags.Component, $"{nameof(ComponentService)}.{nameof(AddComponent)}：未找到该类型{typeof(T).Name}的组件");
            }
        }

        /// <summary>
        /// 递归初始化组件（核心依赖处理逻辑）
        /// 核心逻辑：先收集所有依赖组件（RequireComponent标注的IComponent类型），
        /// 再从栈顶（最底层依赖）开始初始化，保证依赖组件优先完成初始化
        /// </summary>
        /// <param name="entityObject">实体对象容器，传递给组件Init方法（提供初始化上下文）</param>
        /// <param name="currentComponent">当前需要初始化的组件（不可为null）</param>
        /// <returns>初始化完成的组件实例集合（包含当前组件及所有依赖组件）</returns>
        /// <remarks>
        /// 1. 使用栈结构管理初始化顺序：先压入的后初始化（栈先进后出特性）
        /// 2. 仅处理RequireComponent特性中标注的、实现IComponent接口的依赖组件
        /// 3. 每次调用前清空栈，避免跨初始化流程的残留数据导致顺序错误
        /// 4. 组件Init方法由IComponent接口定义，需组件自行实现具体初始化逻辑
        /// </remarks>
        private IEnumerable<IComponent> RecursiveInit(IEntityObject entityObject, IComponent currentComponent)
        {
            // 清空栈：避免上一次初始化的残留数据影响当前流程
            _componentStack.Clear();
            
            var ic = currentComponent;
            while (true)
            {
                // 将当前组件压入栈（先压入的后初始化，保证依赖优先）
                _componentStack.Push(ic);
                // 获取当前组件的RequireComponent特性
                var requireAttr = ic.GetType().GetAttribute<RequireComponent>();
                if (requireAttr != null)
                {
                    // TODO：检查依赖类型是否为IComponent实现类，目前只处理一个参数的重载
                    if (typeof(IComponent).IsAssignableFrom(requireAttr.m_Type0))
                    {
                        // 存在有效依赖组件时，继续递归处理依赖的依赖
                        var component = entityObject.GameObject.GetComponent(requireAttr.m_Type0);  // null
                        if (component is IComponent dependentComponent)
                        {
                            Logger.LogDebug(ELogTags.Component, $"{ic.GetType()}的依赖组件：已找到：{dependentComponent.GetType()}");
                            ic = dependentComponent;
                            continue;
                        }

                        Logger.LogDebug(ELogTags.Component, component ? $"{ic.GetType()}的依赖类型：{requireAttr.m_Type0}，组件：{component.GetType().Name}不为IComponent" : $"{ic.GetType()}的依赖类型：{requireAttr.m_Type0}组件为null");
                    }
                    else
                    {
                        Logger.LogDebug(ELogTags.Component, $"{ic.GetType()}的依赖类型{requireAttr.m_Type0}不从IComponent中派生");
                    }
                }
                else
                {
                    Logger.LogDebug(ELogTags.Component, $"{ic.GetType()}的RequireComponent特性为null");
                }
                
                // 无依赖/依赖非IComponent/依赖未挂载时，开始从栈顶初始化组件
                while (_componentStack.Count > 0)
                {
                    var componentToInit = _componentStack.Pop();
                    // 执行组件自身的初始化逻辑（IComponent接口定义的Init方法）
                    var type = componentToInit.GetType();
                    IComponentCore core = null;
                    var coreAttribute = type.GetCustomAttribute<ComponentCoreAttribute>();
                    if (coreAttribute?.ComponentCore != null)
                    {
                        core = (IComponentCore)DIContainer.Create(coreAttribute.ComponentCore);
                        core.Init(componentToInit);
                    }
                    componentToInit.Init(core);
                    yield return componentToInit;
                }
                
                // 初始化完成，终止循环
                yield break;
            }
        }
        
        /// <summary>
        /// 扫描所有热更组件
        /// </summary>
        private void ScanComponents(Dictionary<string, Type> components, IHotUpdateManager hotUpdateManager)
        {
            // 获取热更的程序集
            foreach (var assembly in hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(IComponent).IsAssignableFrom(type) || type.IsAbstract)
                        continue;
                    
                    var attr = type.GetCustomAttribute<ComponentIdAttribute>();
                    if (attr != null)
                    {
                        components.TryAdd(type.Name, type);
                    }
                }
            }
        }
    }
}