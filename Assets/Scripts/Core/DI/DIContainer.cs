using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.DI
{
    /// <summary>
    /// 依赖容器
    /// </summary>
    public class DIContainer
    {
        // 绑定标志
        private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        // 接口到实例类型映射
        private static readonly ConcurrentDictionary<Type, object> _interfaceMap = new();
        // 实例类型到实例映射
        private static readonly ConcurrentDictionary<Type, object> _instanceMap = new();
        // 新增统一的注入成员缓存
        private static readonly ConcurrentDictionary<Type, List<MemberInfo>> _injectMemberCache = new();
        // 存储接口类型与其默认实现类型的映射（由 BindType 填充）
        private static readonly ConcurrentDictionary<Type, Type> _interfaceToImplTypeMap = new();
        // 记录类型是否为单例（默认为瞬态）,key为实例类型
        private static readonly ConcurrentDictionary<Type, bool> _lifetimes = new();
        // 解析栈，处理循环依赖
        private static readonly ConcurrentStack<Type> _resolveStack = new();
        // 类型构造缓存，缓存能创建该实例类型的所有构造函数
        private static readonly ConcurrentDictionary<Type, List<ConstructorInfo>> _constructorCache = new();
        
        /// <summary>
        /// 绑定类型单例，这个类型只能作为单例使用
        /// </summary>
        /// <typeparam name="TInstance">作为单例的类型</typeparam>
        /// <typeparam name="TInterface">类型接口</typeparam>
        public static void BindSingleton<TInterface, TInstance>() where TInterface : class where TInstance : class, TInterface
        {
            BindType<TInterface, TInstance>();
            _lifetimes.TryAdd(typeof(TInstance), true);
            
            // 注意：此处建议延迟创建实例，改为在首次使用时再创建，避免初始化顺序问题
        }

        /// <summary>
        /// 绑定类型，若注入的类型是接口，则需要在注入前绑定该接口对应的实例类型，否则无法注入接口类型的参数
        /// 若需将绑定的类型作为单例，而使用BindSingleton方法
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <typeparam name="TInstance">实现类型</typeparam>
        public static void BindType<TInterface, TInstance>() where TInterface : class where TInstance : class, TInterface
        {
            if (!typeof(TInterface).IsAssignableFrom(typeof(TInstance)))
                throw new Exception($"{nameof(DIContainer)}.{nameof(BindType)}: {typeof(TInterface).Name} is not assignable from {typeof(TInstance).Name}");
            
            // 记录接口与实现类型的映射
            _interfaceToImplTypeMap.TryAdd(typeof(TInterface), typeof(TInstance));
        }

        /// <summary>
        /// 获取依赖，不存在则自动创建，会递归创建依赖项
        /// </summary>
        /// <param name="type">实例/接口类型</param>
        /// <returns>返回自动创建的实例，若参数为null，则返回null</returns>
        /// <exception cref="Exception">若参数为接口类型，但未在_interfaceToImpl中找到映射则抛出异常</exception>
        private static object Resolve(Type type)
        {
            if (type == null) return null;
            
            if (_resolveStack.Contains(type))
                throw new InvalidOperationException($"Circular dependency detected: {string.Join(" -> ", _resolveStack.Reverse())} -> {type}");
            
            _resolveStack.Push(type);
            try
            {
                // 如果已经是实例（接口或具体类型），直接返回
                if (_interfaceMap.TryGetValue(type, out var existing))
                    return existing;
                if (_instanceMap.TryGetValue(type, out existing))
                    return existing;

                var implType = type;
                // 如果是接口，找到其映射的具体类型
                if (type.IsInterface)
                {
                    // 从映射中查找
                    var find = _interfaceToImplTypeMap.TryGetValue(type, out var itnType);
                    implType = find ? itnType : throw new Exception($"No implementation registered for interface {type.Name}");
                    // 是接口且是Mono
                    if (typeof(Component).IsAssignableFrom(implType))
                    {
                        return CreateMono(type, implType, _lifetimes.GetValueOrDefault(implType));
                    }
                    else
                    {
                        // 先尝试从具体类型缓存中获取已存在的单例实例
                        var cacheInstance = _instanceMap.GetValueOrDefault(implType);
                        if(cacheInstance != null)
                            return _instanceMap.GetValueOrDefault(implType);
                    }
                }
                else
                {
                    // 不是接口且是Mono
                    if (typeof(Component).IsAssignableFrom(implType))
                    {
                        return CreateMono(null, implType, _lifetimes.GetValueOrDefault(type));
                    }
                }
            
                // 不是接口和Mono，解析具体类型，创建类型实例
                return Create(type, implType, _lifetimes.GetValueOrDefault(implType));
            }
            finally
            {
                _resolveStack.TryPop(out _);
            }
        }
        
        /// <summary>
        /// 通过反射创建实例。先尝试构造函数注入（选择参数都能解析的构造函数），再对标记了[Inject]的字段/属性进行补充注入
        /// 若类型已经通过BindSingleton绑定，则忽略参数isSingleton
        /// 若类型已存在单例实例，则返回，否则创建
        /// </summary>
        /// <param name="isSingleton">是否是单例，true创建为单例，false则是瞬态对象</param>
        /// <param name="parameterValues">构造参数值，需按参数顺序匹配，否则无法赋值</param>
        /// <typeparam name="T">非接口引用类型</typeparam>
        /// <returns>类型实例</returns>
        public static T Create<T>(bool isSingleton = false, params object[] parameterValues) where T : class
        {
            var instance = _instanceMap.GetValueOrDefault(typeof(T)) ?? _interfaceMap.GetValueOrDefault(typeof(T));
            if (instance != null)
                return instance as T;
            
            // 创建参数结构数组
            var parameters = new Parameter[parameterValues.Length];
            for (var i = 0; i < parameterValues.Length; i++)
            {
                var parameterValue = parameterValues[i];
                parameters[i] = new Parameter { ArgType = parameterValue.GetType(), ArgValue = parameterValue };
            }
            
            // 先通过构造函数创建实例
            var newInstance = CreateInstanceWithConstructorInjection(typeof(T), parameters);
            // 注入字段/属性
            InjectIntoInstance(newInstance);
            // 先检查是否是绑定单例的具体类型，是的话就忽略isSingleton参数
            var isBindSingleton = _lifetimes.TryGetValue(typeof(T), out var lifetime) && lifetime;
            if (isBindSingleton)
            {
                _instanceMap.TryAdd(typeof(T), newInstance);
                return newInstance as T;
            }
            
            // 没有绑定过，就以参数为准
            // 不是单例直接返回
            if (!isSingleton)
            {
                return (T)newInstance;
            }

            _instanceMap.TryAdd(typeof(T), newInstance);
            return (T)newInstance;
        }

        /// <summary>
        /// 通过反射创建实例。先尝试构造函数注入（选择参数都能解析的构造函数），再对标记了[Inject]的字段/属性进行补充注入
        /// 若类型已经通过BindSingleton绑定，则忽略参数isSingleton
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="instanceType">实例类型</param>
        /// <param name="isSingleton">是否是单例，true创建为单例，false则是瞬态对象</param>
        /// <param name="constructorArgs">构造参数</param>
        /// <returns>新类型实例</returns>
        /// <exception cref="ArgumentException">重复创建单例类型则抛出异常</exception>
        public static object Create(Type interfaceType, Type instanceType, bool isSingleton = false, params Parameter[] constructorArgs)
        {
            if (instanceType == null)
                return null;
            
            var instance = _instanceMap.GetValueOrDefault(instanceType) ?? 
                           (interfaceType != null ? _interfaceMap.GetValueOrDefault(interfaceType) : null);
            if (instance != null)
                return instance;
            
            // 通过构造函数创建实例
            var newInstance = CreateInstanceWithConstructorInjection(instanceType, constructorArgs);
            // 注入字段/属性
            InjectIntoInstance(newInstance);
            // 不是单例直接返回
            if (!isSingleton) return newInstance;
            return _instanceMap.TryAdd(instanceType, newInstance) && (interfaceType == null || _interfaceMap.TryAdd(interfaceType, newInstance)) 
                ? newInstance : throw new ArgumentException($"{interfaceType}-{instanceType} already exists.");
        }

        /// <summary>
        /// 创建mono类型，通过new一个GameObject添加Type类型脚本来实现的
        /// </summary>
        /// <param name="interfaceType">接口类型，可以为null，为null则不存储到_interfaceMap中</param>
        /// <param name="instanceType">实例类型，不能为null</param>
        /// <param name="isSingleton">Mono是否是单例</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">重复创建单例类型抛出异常</exception>
        private static object CreateMono(Type interfaceType, Type instanceType, bool isSingleton)
        {
            var instance = _instanceMap.GetValueOrDefault(instanceType) ?? 
                           (interfaceType != null ? _interfaceMap.GetValueOrDefault(interfaceType) : null);
            
            // 如果实例存在，需要额外判断其是否为有效的 UnityEngine.Object
            if (instance != null)
            {
                // 如果是 UnityEngine.Object 且已被销毁，则清理缓存并视为未命中
                if (instance is Object unityObj && !unityObj)
                {
                    _instanceMap.Remove(instanceType, out _);
                    if (interfaceType != null)
                        _interfaceMap.Remove(interfaceType, out _);
                }
                // 是Unity对象但不为空直接返回
                else
                {
                    return instance;
                }
            }
            
            var go = new GameObject(instanceType.Name);
            if (isSingleton) 
                Object.DontDestroyOnLoad(go);
            var component = go.AddComponent(instanceType);
            InjectIntoInstance(component); // 添加注入
            if (isSingleton)
            {
                _instanceMap.TryAdd(instanceType, component);
                if (interfaceType != null) _interfaceMap.TryAdd(interfaceType, component);
            }
            return component;
        }

        /// <summary>
        /// 通过构造函数创建实例并注入参数依赖
        /// </summary>
        /// <param name="type">必须是实例类型</param>
        /// <param name="parameters">构造函数参数，需按参数匹配</param>
        /// <returns></returns>
        /// <exception cref="Exception">若是接口类型，则抛出异常</exception>
        private static object CreateInstanceWithConstructorInjection(Type type, params Parameter[] parameters)
        {
            Type impType;
            // 接口类型找实例映射
            if (type.IsInterface)
            {
                impType = _interfaceToImplTypeMap.GetValueOrDefault(type);
                if(impType == null)
                    throw new ArgumentException($"Cannot create instance of {type.Name}: no implemented map");
            }
            else
            {
                impType = type;
            }
            
            if (_constructorCache.TryGetValue(impType, out var constructorInfos))
            {
                foreach (var constructorInfo in constructorInfos)
                {
                    var (available, instance) = MatchCtorArg(constructorInfo, parameters);
                    if (available)
                        return instance;
                }
            }
            // 首次创建该类型
            else
            {
                // 获取所有的构造函数
                var constructors = impType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                // 按参数数量降序排序（优先匹配参数最多的构造函数）
                Array.Sort(constructors, (a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));
                // 缓存所有构造
                _constructorCache.TryAdd(impType, new List<ConstructorInfo>(constructors));
                foreach (var constructorInfo in constructors)
                {
                    var (available, instance) = MatchCtorArg(constructorInfo, parameters);
                    if (available)
                    {
                        return instance;
                    }
                } 
            }
            
            // 如果没有合适的构造函数，抛出异常
            throw new Exception($"Cannot create instance of {impType.Name}: no suitable constructor found");
        }

        /// <summary>
        /// 匹配构造参数
        /// </summary>
        /// <param name="ctor">构造信息</param>
        /// <param name="values">参数映射，可为null</param>
        /// <returns>(是否可用，实例)</returns>
        private static (bool available, object instance) MatchCtorArg(ConstructorInfo ctor, Parameter[] values)
        {
            // 获取当前构造所有参数
            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                // 优先使用显式参数中名称匹配的值
                // 存在参数值，且参数类型匹配或是参数值类型从构造参数中派生
                if (values != null && values.Length > 0 && i < values.Length)
                {
                    if(values[i].ArgType == paramType || paramType.IsAssignableFrom(values[i].ArgType))
                    {
                        args[i] = values[i].ArgValue;
                        continue;
                    }
                }

                // 参数值不匹配，尝试从容器中获取依赖
                var value = Resolve(paramType);
                if (value != null)
                {
                    args[i] = value;
                    continue;
                }

                // 如果是可选参数，使用默认值
                if (parameters[i].IsOptional)
                {
                    args[i] = parameters[i].DefaultValue;
                    continue;
                }

                // 无法解析，这个构造函数不可用，用下一个构造函数再次尝试
                return (false, null);
            }

            // 找到可用构造，创建实例
            return (true, ctor.Invoke(args));
        }
        
        /// <summary>
        /// 注入依赖到实例的被Inject修饰的字段/属性
        /// </summary>
        /// <param name="instance">类型实例</param>
        public static void InjectIntoInstance(object instance)
        {
            var type = instance.GetType();
            if (!_injectMemberCache.TryGetValue(type, out var members))
            {
                members = new List<MemberInfo>();
                // 筛选所有带 [Inject] 的字段
                foreach (var field in type.GetFields(_bindingFlags))
                {
                    if (field.IsDefined(typeof(ObsoleteAttribute), true)) continue;
                    if (!Attribute.IsDefined(field, typeof(InjectAttribute))) continue;
                    members.Add(field);
                }
        
                // 筛选所有带 [Inject] 且可写的属性
                foreach (var prop in type.GetProperties(_bindingFlags))
                {
                    if (prop.IsDefined(typeof(ObsoleteAttribute), true)) continue;
                    if (!Attribute.IsDefined(prop, typeof(InjectAttribute))) continue;
                    if (!prop.CanWrite) continue;
                    members.Add(prop);
                }
        
                _injectMemberCache[type] = members;
            }
    
            // 遍历缓存成员，执行注入
            foreach (var member in members)
            {
                switch (member)
                {
                    case FieldInfo field:
                        if (field.GetValue(instance) != null) 
                            continue;
                        var fieldValue = Resolve(field.FieldType);
                        if (fieldValue != null)
                            field.SetValue(instance, fieldValue);
                        else
                            Debug.LogWarning($"{type} 的字段 {field.Name} 未找到依赖项");
                        break;
                    case PropertyInfo prop:
                        if (prop.CanRead && prop.GetValue(instance) != null) 
                            continue;
                        var propValue = Resolve(prop.PropertyType);
                        if (propValue != null)
                            prop.SetValue(instance, propValue);
                        else
                            Debug.LogWarning($"{type} 的属性 {prop.Name} 未找到依赖项");
                        break;
                }
            }
        }

        /// <summary>
        /// 获取实例，未找到返回null
        /// </summary>
        /// <typeparam name="T">接口类型/实例类型</typeparam>
        /// <returns>未找到返回null</returns>
        public static T GetInstance<T>() where T : class
        {
            var type = typeof(T);
            if (_interfaceMap.TryGetValue(type, out var obj) && obj is T t1)
                return t1;
            if (_instanceMap.TryGetValue(type, out obj) && obj is T t2)
                return t2;
            return null;
        }
        
        /// <summary>
        /// 解绑接口对该类型的映射，且从缓存中移除实例，移除后需重新BindSingleton或BindType
        /// </summary>
        /// <typeparam name="TInstance">具体类型</typeparam>
        /// <typeparam name="TInterface">若实现类型存在接口，则传入对应接口</typeparam>
        public static void Unbind<TInstance, TInterface>()
        {
            var implementationType = typeof(TInstance);
            var interfaceType = typeof(TInterface);

            var interfaces = implementationType.GetInterfaces();
            foreach (var face in interfaces)
            {
                _interfaceToImplTypeMap.Remove(face, out _);
                _interfaceMap.Remove(face, out _);
            }
            
            _interfaceToImplTypeMap.Remove(interfaceType, out _);
            _interfaceMap.Remove(interfaceType, out _);
            _lifetimes.Remove(implementationType, out _);
            _instanceMap.Remove(implementationType, out _);
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public static void Clear()
        {
            _interfaceMap.Clear();
            _instanceMap.Clear();
            _interfaceToImplTypeMap.Clear();
            _lifetimes.Clear();
            _resolveStack.Clear();
            _constructorCache.Clear();
            _injectMemberCache.Clear();
        }
    }
}
