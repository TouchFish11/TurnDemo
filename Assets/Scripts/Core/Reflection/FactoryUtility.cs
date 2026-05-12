using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Components;
using Core.DI;
using Core.HotUpdate;
using Core.Log;

namespace Core.Reflection
{
    /// <summary>
    /// 工厂工具类
    /// </summary>
    public static class FactoryUtility
    {
        /// <summary>
        /// 批量扫描指定程序集中所有实现/继承TIValue的类型，并通过自定义委托将类型转换为键值对缓存到字典中
        /// </summary>
        /// <typeparam name="TIValue">筛选基准类型（接口/抽象类/普通类），仅扫描可赋值给该类型的类</typeparam>
        /// <typeparam name="Tkey">字典的键类型，由keyFunc委托决定具体值</typeparam>
        /// <typeparam name="TValue">字典的值类型（约束为引用类型），由valueFunc委托决定具体值</typeparam>
        /// <param name="dic">用于缓存扫描结果的字典，方法内部会直接向该字典添加键值对</param>
        /// <param name="keyFunc">类型转字典键的委托，入参为符合条件的Type，返回对应Tkey类型的键</param>
        /// <param name="valueFunc">类型转字典值的委托，入参为符合条件的Type，返回对应TValue类型的值</param>
        /// <param name="isAbstract">是否包含抽象类，默认false（不包含）</param>
        /// <param name="isInterface">是否包含接口类型，默认false（不包含）</param>
        /// <param name="assemblies">要扫描的程序集数组，为空时不会执行任何扫描逻辑</param>
        /// <exception cref="ArgumentNullException">当dic/keyFunc/valueFunc/assemblies为null，或assemblies包含null元素时抛出</exception>
        /// <exception cref="ArgumentException">当keyFunc返回重复键导致字典添加失败时抛出</exception>
        /// <remarks>
        /// 核心逻辑说明：
        /// 1. 遍历传入的每个程序集，获取所有定义的类型；
        /// 2. 筛选规则：
        ///    - 类型可赋值给TIValue（即实现接口/继承基类）；
        ///    - 根据isAbstract决定是否包含抽象类；
        ///    - 根据isInterface决定是否包含接口类型；
        /// 3. 通过自定义委托将符合条件的类型转换为键值对，添加到字典中；
        /// 4. 泛型约束：TValue必须是引用类型，避免值类型的装箱/拆箱开销。
        /// 注意事项：
        /// 1. 字典需提前初始化，方法不会自动创建字典；
        /// 2. 若存在重复键，会抛出ArgumentException，建议使用TryAdd逻辑（可根据需求修改方法）；
        /// 3. 扫描大量程序集时可能有性能开销，建议在程序启动时执行一次并缓存结果；
        /// 4. 不扫描嵌套类型（如需支持可修改assembly.GetTypes()为递归遍历）。
        /// </remarks>
        public static void ScanAllType<TIValue, Tkey, TValue>(IDictionary<Tkey, TValue> dic, Func<Type, Tkey> keyFunc, Func<Type, TValue> valueFunc, bool isAbstract = false, bool isInterface = false, params Assembly[] assemblies) where TValue : class
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic), "缓存字典不能为空");
            if (keyFunc == null)
                throw new ArgumentNullException(nameof(keyFunc), "类型转键的委托不能为空");
            if (valueFunc == null)
                throw new ArgumentNullException(nameof(valueFunc), "类型转值的委托不能为空");
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies), "要扫描的程序集数组不能为空");

            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                {
                    throw new ArgumentNullException(nameof(assembly), "程序集数组中包含null元素");
                }

                foreach (var type in assembly.GetTypes())
                {
                    var isTypeMatch = typeof(TIValue).IsAssignableFrom(type);
                    var isAbstractMatch = isAbstract ? type.IsAbstract : !type.IsAbstract;
                    var isInterfaceMatch = isInterface ? type.IsInterface : !type.IsInterface;

                    if (isTypeMatch && isAbstractMatch && isInterfaceMatch)
                    {
                        dic.Add(keyFunc.Invoke(type), valueFunc.Invoke(type));
                    }
                }
            }
        }
        
        /// <summary>
        /// 扫描所有实现TIValue的类型
        /// </summary>
        public static void ScanAllType<TIValue>(Dictionary<Type, TIValue> dic, params Assembly[] assemblies) where TIValue : class
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(TIValue).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    {
                        dic.Add(type, DIContainer.Create(null, type) as TIValue);
                    }
                }
            }
        }

        /// <summary>
        /// 扫描所有实现IFactory的工厂
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="assemblies"></param>
        public static void ScanAllFactory<TValue>(Dictionary<Type, TValue> dic, params Assembly[] assemblies) where TValue : class, IFactory
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(TValue).IsAssignableFrom(type) || type.IsAbstract)
                    {
                        continue;
                    }
                    
                    // 通过DI创建类型
                    var factory = DIContainer.Create(null, type) as TValue;
                    factory?.InitFactory();
                    if (!dic.TryAdd(type, factory))
                    {
                        Logger.LogError($"{nameof(FactoryUtility)}.{nameof(ScanAllFactory)}：重复添加工厂类型：{type}");
                    }
                }
            }
        }

        /// <summary>
        /// 扫描所有热更组件
        /// </summary>
        public static void ScanComponents(IDictionary<string, Type> components)
        {
            // 获取热更的程序集
            foreach (var assembly in DIContainer.GetInstance<IHotUpdateManager>().GetHotAssemblies())
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
