using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Game.Battle;
using System;
using System.Reflection;

/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : SingletonBase<SkillManager>, ISkillManager
{
    // 类型到条件映射
    private static readonly Dictionary<Type, ICastSkillCondition> typeToConditionMap = new Dictionary<Type, ICastSkillCondition>();

    static SkillManager()
    {
        ScanAllCastSkillCondition();
    }

    private SkillManager()
    {

    }

    /// <summary>
    /// 添加技能命令到回合队列
    /// </summary>
    public void AddSkillCommand(ISkill skill, IBattleEntityObject entityObject)
    {
        // 获取技能释放对象  待优化：应为触发技能的实体对象，而不一定是当前回合实体
        IBattleEntityObject caster = entityObject;
        // 通过目标选择管理器获取技能主目标
        IBattleEntityObject mainTaget = ServiceLocator.Instance.Get<ITargetSelectManager>().GetMainTarget();
        // 通过目标选择管理器获取技能所有目标
        List<IBattleEntityObject> selectedTargets = ServiceLocator.Instance.Get<ITargetSelectManager>().GetTargets();
        // 初始化技能
        skill.Init(caster, mainTaget, selectedTargets);
        // 放入指令
        ServiceLocator.Instance.Get<IBattleManager>().GetContext().GetTurnManager().EnqueueCommand(skill);
    }

    /// <summary>
    /// 获取技能释放条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ICastSkillCondition GetCastSkillCondition<T>() where T : class, ICastSkillCondition
    {
        if (typeToConditionMap.TryGetValue(typeof(T), out ICastSkillCondition condition))
        {
            return condition as T;
        }

        LogManager.LogError($"未找到技能释放条件：{typeof(T)}");
        return null;
    }

    /// <summary>
    /// 扫描所有释放技能条件
    /// </summary>
    private static void ScanAllCastSkillCondition()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            CastSkillConditionAttribute attribute = type.GetCustomAttribute<CastSkillConditionAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (typeof(ICastSkillCondition).IsAssignableFrom(type))
            {
                typeToConditionMap.Add(type, Activator.CreateInstance(type) as ICastSkillCondition);
            }
        }
    }
}
