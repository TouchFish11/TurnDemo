using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.DI
{
    public static class DIContainer
    {
        private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        // 瞬态映射：服务类型->实现类型
        private static readonly Dictionary<Type, Type> _transientMap = new();

        // 单例映射：服务类型->BindingInfo，包含实现类型与缓存实例
        private static readonly Dictionary<Type, BindingInfo> _singletonMap = new();

        // 构造缓存
        private static readonly ConcurrentDictionary<Type, List<ConstructorInfo>> _constructorCache = new();
        private static readonly ConcurrentDictionary<Type, List<MemberInfo>> _injectMemberCache = new();
        // 解析栈
        private static readonly Stack<Type> _resolveStack = new();
    
        /// <summary>
        /// 绑定单例，支持多接口分开调用自动合并
        /// </summary>
        public static void BindSingleton<TInterface, TInstance>() where TInstance : class, TInterface
        {
            var interfaceType = typeof(TInterface);
            var implType = typeof(TInstance);

            BindingInfo existingInfo = null; 
            // 查找是否已有相同实现类型的单例绑定
            foreach (var bindingInfo in _singletonMap.Values)
            {
                if (bindingInfo.ImplementationType != implType) 
                    continue;
            
                existingInfo = bindingInfo;
                break;
            }
        
            if (existingInfo != null)
            {
                // 合并：将新接口指向已有的 BindingInfo
                _singletonMap.TryAdd(interfaceType, existingInfo);
                // 确保具体类型也指向它（如果还没有的话）
                _singletonMap.TryAdd(implType, existingInfo);
                return;
            }

            // 新建单例绑定
            var info = new BindingInfo { ImplementationType = implType };
            _singletonMap[interfaceType] = info;
            // 支持 Resolve<Player>
            _singletonMap[implType] = info;
        }

        /// <summary>
        /// 绑定单例，支持多接口分开调用自动合并
        /// </summary>
        public static void BindSingleton(Type interfaceType, Type implType)
        {
            BindingInfo existingInfo = null; 
            // 查找是否已有相同实现类型的单例绑定
            foreach (var bindingInfo in _singletonMap.Values)
            {
                if (bindingInfo.ImplementationType != implType) 
                    continue;
            
                existingInfo = bindingInfo;
                break;
            }
        
            if (existingInfo != null)
            {
                // 合并：将新接口指向已有的 BindingInfo
                _singletonMap.TryAdd(interfaceType, existingInfo);
                // 确保具体类型也指向它（如果还没有的话）
                _singletonMap.TryAdd(implType, existingInfo);
                return;
            }

            // 新建单例绑定
            var info = new BindingInfo { ImplementationType = implType };
            _singletonMap[interfaceType] = info;
            // 支持 Resolve<Player>
            _singletonMap[implType] = info;
        }
    
        /// <summary>
        /// 绑定瞬态
        /// </summary>
        public static void BindType<TInterface, TInstance>() where TInstance : class, TInterface
        {
            var interfaceType = typeof(TInterface);
            var implType = typeof(TInstance);
            _transientMap[interfaceType] = implType;
            // 也允许直接解析具体类型，方便隐式解析
            _transientMap.TryAdd(implType, implType);
        }

        /// <summary>
        /// 解析类型返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>() where T : class
        {
            return ResolveInternal(typeof(T)) as T;
        }

        /// <summary>
        /// 解析类型返回对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Resolve(Type type)
        {
            return ResolveInternal(type);
        }

        /// <summary>
        /// 流式绑定入口，支持一次性绑定多个接口
        /// </summary>
        public static BindingBuilder<T> Bind<T>() where T : class
        {
            return new BindingBuilder<T>();
        }
    
        internal static void RegisterSingleton(BindingInfo info, List<Type> serviceTypes)
        {
            BindingInfo existing = null; 
            foreach (var bindingInfo in _singletonMap.Values)
            {
                if (bindingInfo.ImplementationType != info.ImplementationType) 
                    continue;
            
                existing = bindingInfo;
                break;
            }
        
            if (existing != null)
            {
                foreach (var st in serviceTypes)
                {
                    _singletonMap.TryAdd(st, existing);
                }
                return;
            }

            foreach (var st in serviceTypes)
            {
                _singletonMap.Add(st, info);
            }
        }

        internal static void RegisterTransient(Type serviceType, Type implType)
        {            
            _transientMap.Add(serviceType, implType);
        }

        public static T GetInstance<T>() where T : class
        {
            return ResolveInternal(typeof(T)) as T;
        }

        private static object ResolveInternal(Type serviceType)
        {
            if (serviceType == null) 
                return null;

            if (_resolveStack.Contains(serviceType))
                throw new InvalidOperationException($"Circular dependency: {string.Join(" -> ", _resolveStack.Reverse())} -> {serviceType}");

            // 检查单例
            if (_singletonMap.TryGetValue(serviceType, out var bindingInfo))
            {
                // Unity 对象销毁检测
                if (bindingInfo.CachedInstance != null)
                {
                    if (bindingInfo.CachedInstance is Object unityObj && !unityObj)
                        bindingInfo.CachedInstance = null;
                    else
                        return bindingInfo.CachedInstance;
                }

                _resolveStack.Push(serviceType);
                try
                {
                    var instance = Instantiate(bindingInfo.ImplementationType);
                    bindingInfo.CachedInstance = instance;
                    return instance;
                }
                finally
                {
                    _resolveStack.Pop();
                }
            }

            // 检查瞬态
            if (_transientMap.TryGetValue(serviceType, out var implType))
            {
                _resolveStack.Push(serviceType);
                try
                {
                    return Instantiate(implType);
                }
                finally
                {
                    _resolveStack.Pop();
                }
            }

            // 隐式解析具体类（非接口/抽象）
            if (!serviceType.IsInterface && !serviceType.IsAbstract)
            {
                _resolveStack.Push(serviceType);
                try
                {
                    return Instantiate(serviceType);
                }
                finally
                {
                    _resolveStack.Pop();
                }
            }

            throw new Exception($"Type {serviceType.Name} is not registered and cannot be implicitly created.");
        }
    
        /// <summary>
        /// 实例创建，整合 MonoBehaviour 与普通类
        /// </summary>
        /// <param name="implType"></param>
        /// <param name="explicitParams"></param>
        /// <returns></returns>
        private static object Instantiate(Type implType, Parameter[] explicitParams = null)
        {
            object instance;
            if (typeof(Component).IsAssignableFrom(implType))
            {
                var go = new GameObject(implType.Name);
                var comp = go.AddComponent(implType);
                instance = comp;
            }
            else
            {
                instance = CreateInstanceWithConstructorInjection(implType, explicitParams ?? Array.Empty<Parameter>());
            }

            // 特性注入
            InjectIntoInstance(instance);
            return instance;
        }
    
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static object CreateInstanceWithConstructorInjection(Type type, params Parameter[] parameters)
        {
            if (_constructorCache.TryGetValue(type, out var constructors))
            {
                foreach (var ctor in constructors)
                {
                    var (ok, result) = MatchCtorArg(ctor, parameters);
                    if (ok)
                    {
                        return result;
                    }
                }
            }
            else
            {
                var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                // 降序
                Array.Sort(ctors, (a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));
                _constructorCache.TryAdd(type, new List<ConstructorInfo>(ctors));
                foreach (var ctor in ctors)
                {
                    var (ok, result) = MatchCtorArg(ctor, parameters);
                    if (ok)
                    {
                        return result;
                    }
                }
            }

            throw new Exception($"[{nameof(DIContainer)}]: Cannot create instance of {type.Name}, no suitable constructor.");
        }

        /// <summary>
        /// 匹配构造参数
        /// </summary>
        /// <param name="ctor"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static (bool ok, object instance) MatchCtorArg(ConstructorInfo ctor, Parameter[] values)
        {
            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;

                if (values != null && i < values.Length && (values[i].ArgType == paramType || paramType.IsAssignableFrom(values[i].ArgType)))
                {
                    args[i] = values[i].ArgValue;
                    continue;
                }

                var resolved = ResolveInternal(paramType);
                if (resolved != null)
                {
                    args[i] = resolved;
                    continue;
                }

                if (parameters[i].IsOptional)
                {
                    args[i] = parameters[i].DefaultValue;
                    continue;
                }

                return (false, null);
            }

            return (true, ctor.Invoke(args));
        }

        /// <summary>
        /// 字段、属性的Inject特性注入
        /// </summary>
        /// <param name="instance"></param>
        public static void InjectIntoInstance(object instance)
        {
            var type = instance.GetType();
            if (!_injectMemberCache.TryGetValue(type, out var members))
            {
                members = new List<MemberInfo>();
                foreach (var field in type.GetFields(_bindingFlags))
                {
                    if (field.IsDefined(typeof(ObsoleteAttribute), true)) continue;
                    if (Attribute.IsDefined(field, typeof(InjectAttribute)))
                        members.Add(field);
                }
                foreach (var prop in type.GetProperties(_bindingFlags))
                {
                    if (prop.IsDefined(typeof(ObsoleteAttribute), true)) continue;
                    if (Attribute.IsDefined(prop, typeof(InjectAttribute)) && prop.CanWrite)
                        members.Add(prop);
                }
                _injectMemberCache[type] = members;
            }

            foreach (var member in members)
            {
                switch (member)
                {
                    case FieldInfo field:
                        if (field.GetValue(instance) == null)
                        {
                            var val = ResolveInternal(field.FieldType);
                            if (val != null) field.SetValue(instance, val);
                        }
                        break;
                    case PropertyInfo prop:
                        if (prop.GetValue(instance) == null)
                        {
                            var val = ResolveInternal(prop.PropertyType);
                            if (val != null) prop.SetValue(instance, val);
                        }
                        break;
                }
            }
        }
        
        /// <summary>
        /// 创建方法：替代 new，并自动注入依赖
        /// </summary>
        /// <param name="parameterValues">可选：手动指定的构造参数（按顺序）</param>
        public static T Create<T>(params object[] parameterValues) where T : class
        {
            var type = typeof(T);
            // 如果是接口或抽象类，无法 new，直接抛出明确错误
            if (type.IsInterface || type.IsAbstract)
                throw new ArgumentException($"Cannot create instance of {type.Name} because it is an interface or abstract class. Use GetInstance<T>() for resolved instances.");
    
            // 构造参数包装
            var parameters = new Parameter[parameterValues.Length];
            for (var i = 0; i < parameterValues.Length; i++)
            {
                parameters[i] = new Parameter { ArgType = parameterValues[i].GetType(), ArgValue = parameterValues[i] };
            }
    
            // 直接创建实例（Instantiate 内部会处理构造函数注入和 [Inject] 字段/属性）
            var instance = Instantiate(type, parameters);
            return instance as T;
        }

        /// <summary>
        /// 创建方法：替代 new，并自动注入依赖
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">可选：手动指定的构造参数（按顺序）</exception>
        public static object Create(Type type, params object[] parameterValues)
        {
            // 如果是接口或抽象类，无法 new，直接抛出明确错误
            if (type.IsInterface || type.IsAbstract)
                throw new ArgumentException($"Cannot create instance of {type.Name} because it is an interface or abstract class. Use GetInstance<T>() for resolved instances.");
    
            // 构造参数包装
            var parameters = new Parameter[parameterValues.Length];
            for (int i = 0; i < parameterValues.Length; i++)
            {
                parameters[i] = new Parameter { ArgType = parameterValues[i].GetType(), ArgValue = parameterValues[i] };
            }
    
            // 直接创建实例（Instantiate 内部会处理构造函数注入和 [Inject] 字段/属性）
            var instance = Instantiate(type, parameters);
            return instance;
        }
    
        /// <summary>
        /// 统一移除绑定方法
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TInterface"></typeparam>
        public static void Unbind<TInstance, TInterface>()
        {
            var implType = typeof(TInstance);
            var interfaceType = typeof(TInterface);

            // 移除单例
            var keysToRemove = new List<Type>();
            foreach (var singletonMapKey in _singletonMap.Keys)
            {
                if (singletonMapKey == interfaceType || singletonMapKey == implType)
                {
                    keysToRemove.Add(singletonMapKey);
                }
            }

            foreach (var key in keysToRemove)
            {
                _singletonMap.Remove(key);
            }

            // 移除瞬态
            _transientMap.Remove(interfaceType);
            _transientMap.Remove(implType);
        }

        public static void Clear()
        {
            _singletonMap.Clear();
            _transientMap.Clear();
            _constructorCache.Clear();
            _injectMemberCache.Clear();
            _resolveStack.Clear();
        }
    }
}