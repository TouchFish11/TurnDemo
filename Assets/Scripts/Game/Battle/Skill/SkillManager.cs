using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Game.Battle;

/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : SingletonBase<SkillManager>, ISkillManager
{
    private SkillManager()
    {

    }

    /// <summary>
    /// 添加技能命令到回合队列
    /// </summary>
    public void AddSkillCommand(ISkill skill, IBattleEntityObject entityObject)
    {
        // 获取上下文
        IBattleContext battleContext = entityObject.Context;
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
}
