using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Game.Battle;

/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : SingletonBase<SkillManager>
{
    private SkillManager()
    {

    }

    /// <summary>
    /// 添加技能命令到回合队列
    /// </summary>
    /// <param name="skillInfo"></param>
    /// <param name="roleInfo"></param>
    public void AddSkillCommand(ISkill skill, IBattleEntityObject entityObject)
    {
        // 获取上下文
        IBattleContext battleContext = BattleManager.Instance.GetContext();
        // 获取技能释放对象  待优化：应为触发技能的实体对象，而不一定是当前回合实体
        IBattleEntityObject caster = battleContext.GetTurnManager().GetCurrentEntity();
        // 通过目标选择管理器获取技能主目标
        IBattleEntityObject mainTaget = TargetSelectManager.Instance.GetMainTarget();
        // 通过目标选择管理器获取技能所有目标
        List<IBattleEntityObject> selectedTargets = TargetSelectManager.Instance.GetTargets();
        // 初始化技能
        skill.Init(caster, mainTaget, selectedTargets);
        BattleManager.Instance.GetContext().GetTurnManager().EnqueueCommand(skill);
    }

    private List<IBattleEntityObject> FindTargets(SkillInfo skillInfo, IBattleContext context, IBattleEntityObject mainTarget)
    {
        List<IBattleEntityObject> targets = null;

        // 判断技能目标类型
        switch ((E_SkillTargetType)skillInfo.f_SkillTargetType)
        {
            case E_SkillTargetType.Friend:
                targets = new List<IBattleEntityObject>(context.GetPlayerObjects());
                break;
            case E_SkillTargetType.Enemy:
                targets = new List<IBattleEntityObject>(context.GetMonsterObjects());
                break;
        }

        // 根据技能范围类型查找目标
        switch ((E_SkillRangeType)skillInfo.f_skillRangeType)
        {
            case E_SkillRangeType.Singel:
                return new List<IBattleEntityObject>() { mainTarget };
            case E_SkillRangeType.Diffusion:
                // 通过战斗上下文获取扩散范围内的目标（示例代码，需根据实际逻辑实现）
                return new List<IBattleEntityObject>(targets);
            case E_SkillRangeType.All:
                return new List<IBattleEntityObject>(targets);
        }

        return new List<IBattleEntityObject>();
    }
}
