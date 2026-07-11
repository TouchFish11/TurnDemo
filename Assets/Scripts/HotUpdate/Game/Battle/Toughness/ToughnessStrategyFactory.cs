using System;
using System.Collections.Generic;
using System.Reflection;
using Core.HotUpdate;
using Core.Log;

namespace HotUpdate.Game.Battle.Toughness
{
    /// <summary>
    /// 韧性策略工厂类
    /// 负责扫描、注册和获取不同类型的韧性策略实例
    /// </summary>
    public class ToughnessStrategyFactory : IToughnessStrategyFactory
    {
        // 缓存容器：键为韧性策略类型，值为对应的策略类型与实例的集合
        private static readonly Dictionary<E_ToughnessStrategyType, List<(Type, object)>> typeToToughnessMap = new();

        private ToughnessStrategyFactory(IHotUpdateManager hotUpdateManager)
        {
            ScanAllToughnessStrategy(hotUpdateManager);
        }
        
        public IToughnessReduceStrategy GetReduceStrategy<T>() where T : class, IToughnessReduceStrategy
        {
            // 从缓存中获取削减判定类型的策略集合
            if (typeToToughnessMap.TryGetValue(E_ToughnessStrategyType.ReduceJudge, out var list))
            {
                // 遍历集合匹配目标类型
                foreach (var (typeIdentifier, strategy) in list)
                {
                    if (typeof(T) == typeIdentifier)
                    {
                        return strategy as T;
                    }
                }
            }

            Logger.LogError(ELogTags.Battle, $"未注册的韧性削减策略类型：{typeof(T)}");
            return null;
        }

        public IToughnessCalcStrategy GetCalcStrategy<T>() where T : class, IToughnessCalcStrategy
        {
            // 从缓存中获取数值计算类型的策略集合
            if (typeToToughnessMap.TryGetValue(E_ToughnessStrategyType.ValueCalculate, out var list))
            {
                // 遍历集合匹配目标类型
                foreach (var (typeIdentifier, strategy) in list)
                {
                    if (typeof(T) == typeIdentifier)
                    {
                        return strategy as T;
                    }
                }
            }

            Logger.LogError(ELogTags.Battle, $"未注册的韧性数值计算策略类型：{typeof(T)}");
            return null;
        }

        /// <summary>
        /// 扫描并注册所有韧性策略
        /// 遍历当前程序集所有类型，筛选带有ToughnessStrategyAttribute特性的类型，
        /// 根据策略类型创建对应实例并缓存
        /// </summary>
        /// <param name="hotUpdateManager"></param>
        private static void ScanAllToughnessStrategy(IHotUpdateManager hotUpdateManager)
        {
            foreach (var hotUpdateAssembly in hotUpdateManager.GetHotAssemblies())
            {
                // 遍历当前执行程序集中的所有类型
                foreach (var type in hotUpdateAssembly.GetTypes())
                {
                    // 获取类型上标记的韧性策略特性
                    var attr = type.GetCustomAttribute<ToughnessStrategyAttribute>();
                    if (attr == null)
                    {
                        // 无该特性则跳过当前类型
                        continue;
                    }

                    // 根据策略类型创建对应的策略实例
                    var strategyInstance = attr.StrategyType switch
                    {
                        // 减伤判定类型：需实现IToughnessReduceStrategy接口
                        E_ToughnessStrategyType.ReduceJudge when typeof(IToughnessReduceStrategy).IsAssignableFrom(type) =>
                            Activator.CreateInstance(type),
                        // 数值计算类型：需实现IToughnessCalcStrategy接口
                        E_ToughnessStrategyType.ValueCalculate when typeof(IToughnessCalcStrategy).IsAssignableFrom(type) =>
                            Activator.CreateInstance(type),
                        // 不匹配的策略类型返回null
                        _ => null
                    };
                    
                    // 将特性中的优先级赋值给策略实例的优先级属性
                    strategyInstance?.GetType().GetProperty(nameof(attr.Priority))?.SetValue(strategyInstance, attr.Priority);

                    // 将策略类型与实例添加到缓存容器
                    if (typeToToughnessMap.TryGetValue(attr.StrategyType, out var list))
                    {
                        list.Add((type, strategyInstance));
                    }
                    else
                    {
                        typeToToughnessMap.Add(attr.StrategyType, new List<(Type, object)> { (type, strategyInstance) });
                    }
                }
            }
        }
    }
}