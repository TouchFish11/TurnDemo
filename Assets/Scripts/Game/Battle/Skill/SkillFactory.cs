using Framework;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 技能工厂
/// </summary>
public abstract class SkillFactory : ISkillFactory
{
    private readonly static Dictionary<Type, ISkillCastPostHandler> typeToHandlerMap = new Dictionary<Type, ISkillCastPostHandler>();

    static SkillFactory()
    {
        ScanAllSkillCastPostHandler();
    }

    /// <summary>
    /// 批量创建技能对象
    /// </summary>
    /// <param name="skillIds"></param>
    /// <returns></returns>
    public IEnumerable<ISkill> CreateSkills(IBattleEntityObject caster, params int[] skillIds)
    {
        List<ISkill> skills = new List<ISkill>();
        foreach (int skillId in skillIds)
        {
            skills.Add(CreateSkill(caster, skillId));
        }
        return skills;
    }

    /// <summary>
    /// 创建技能对象
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public abstract ISkill CreateSkill(IBattleEntityObject caster, int skillId);

    /// <summary>
    /// 获取技能释放后处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ISkillCastPostHandler GetSkillCastPostHandler<T>() where T : class, ISkillCastPostHandler
    {
        if (typeToHandlerMap.TryGetValue(typeof(T), out ISkillCastPostHandler handler))
        {
            return handler as T;
        }

        LogManager.LogError($"未找到技能释放后处理器：{typeof(T)}");
        return null;
    }

    private static void ScanAllSkillCastPostHandler()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            SkillCastPostHandlerAttribute attribute = type.GetCustomAttribute<SkillCastPostHandlerAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (typeof(ISkillCastPostHandler).IsAssignableFrom(type))
            {
                typeToHandlerMap.Add(type, Activator.CreateInstance(type) as ISkillCastPostHandler);
            }
        }
    }
}
