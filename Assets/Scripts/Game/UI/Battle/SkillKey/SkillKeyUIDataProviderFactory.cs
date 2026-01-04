using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SkillKeyUIDataProviderFactory
{
    private static readonly Dictionary<Type, ISkillKeyUIDataProvider> typeToProviderMap = new Dictionary<Type, ISkillKeyUIDataProvider>();

    static SkillKeyUIDataProviderFactory()
    {
        ScanAllSkillKeyUIDataProvider();
    }


    private static void ScanAllSkillKeyUIDataProvider()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            SkillKeyUIDataProviderAttribute attribute = type.GetCustomAttribute<SkillKeyUIDataProviderAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (typeof(ISkillKeyUIDataProvider).IsAssignableFrom(type))
            {
                typeToProviderMap.Add(type, Activator.CreateInstance(type) as ISkillKeyUIDataProvider);
            }
        }
    }

    public static ISkillKeyUIDataProvider GetProvider<T>() where T : class, ISkillKeyUIDataProvider
    {
        if (typeToProviderMap.TryGetValue(typeof(T), out ISkillKeyUIDataProvider provider))
        {
            return provider;
        }

        LogManager.LogError($"未找到技能按键UI数据提供器：{typeof(T)}");
        return null;
    }

}
