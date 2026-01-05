using Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 韧性策略工厂
/// </summary>
public class ToughnessStrategyFactory
{
    // 缓存：规则类型 → 规则实例列表
    private static readonly Dictionary<E_ToughnessStrategyType, List<(Type, object)>> typeToToughnessMap = new Dictionary<E_ToughnessStrategyType, List<(Type, object)>>();

    static ToughnessStrategyFactory()
    {
        ScanAllToughnessStrategy();
    }

    /// <summary>
    /// 扫描所有韧性相关策略
    /// </summary>
    private static void ScanAllToughnessStrategy()
    {
        // 扫描当前程序集所有类
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            // 获取特性
            ToughnessStrategyAttribute attr = type.GetCustomAttribute<ToughnessStrategyAttribute>();
            if (attr == null)
            {
                continue;
            }

            object strategyInstance = null;
            if (attr.StrategyType == E_ToughnessStrategyType.ReduceJudge && typeof(IToughnessReduceStrategy).IsAssignableFrom(type))
            {
                // 实例化策略
                strategyInstance = Activator.CreateInstance(type);
            }
            else if (attr.StrategyType == E_ToughnessStrategyType.ValueCalculate && typeof(IToughnessCalcStrategy).IsAssignableFrom(type))
            {
                // 实例化策略
                strategyInstance = Activator.CreateInstance(type);
            }

            // 反射赋值优先级
            strategyInstance.GetType().GetProperty(nameof(attr.Priority)).SetValue(strategyInstance, attr.Priority);

            if (typeToToughnessMap.TryGetValue(attr.StrategyType, out List<(Type, object)> list))
            {
                list.Add((type, strategyInstance));
            }
            else
            {
                typeToToughnessMap.Add(attr.StrategyType, new List<(Type, object)>() { (type, strategyInstance) });
            }
        }
    }

    /// <summary>
    /// 获取韧性削减策略
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IToughnessReduceStrategy GetReduceStrategy<T>() where T : class, IToughnessReduceStrategy
    {
        if (typeToToughnessMap.TryGetValue(E_ToughnessStrategyType.ReduceJudge, out var list))
        {
            foreach ((Type type, object strategy) in list)
            {
                if (type == typeof(T))
                {
                    return strategy as T;
                }
            }
        }

        LogManager.LogError($"未注册韧性削减策略类型：{typeof(T)}");
        return null;
    }

    /// <summary>
    /// 获取韧性计算策略
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IToughnessCalcStrategy GetCalcStrategy<T>() where T : class, IToughnessCalcStrategy
    {
        if (typeToToughnessMap.TryGetValue(E_ToughnessStrategyType.ValueCalculate, out var list))
        {
            foreach ((Type type, object strategy) in list)
            {
                if (type == typeof(T))
                {
                    return strategy as T;
                }
            }
        }

        LogManager.LogError($"未注册韧性计算策略类型：{typeof(T)}");
        return null;
    }
}
