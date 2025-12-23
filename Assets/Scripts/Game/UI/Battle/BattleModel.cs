using Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 战斗界面数据
/// </summary>
public class BattleModel : UIModel
{
    // 行动条格子UI列表
    private readonly List<ActionGridUI> actions = new List<ActionGridUI>();
    // 技能按键UI列表
    private readonly List<SkillKeyUI> skillKeyUIs = new List<SkillKeyUI>();
    // 角色状态UI列表
    private readonly List<RoleStateUI> roleStateUIs = new List<RoleStateUI>();
    // 战技点UI列表
    private readonly List<BattlePointUI> battlePointUIs = new List<BattlePointUI>();

    public void UpdateAcitonbar(IEnumerable<ActionGridUI> actionGridUIs)
    {
        foreach (ActionGridUI actionGridUI in actions)
        {
            PoolManager.Instance.PushObj(actionGridUI.gameObject);
        }
        actions.Clear();

        actions.AddRange(actionGridUIs);
        TriggerDataChanged(nameof(actions), actions);
    }

    public void UpdateOperator(IEnumerable<SkillKeyUI> skillKeyUIs)
    {
        foreach (SkillKeyUI skillKeyUI in this.skillKeyUIs)
        {
            PoolManager.Instance.PushObj(skillKeyUI.gameObject);
        }
        this.skillKeyUIs.Clear();

        this.skillKeyUIs.AddRange(skillKeyUIs);
        TriggerDataChanged(nameof(this.skillKeyUIs), skillKeyUIs);
    }

    public void UpdateBattlePointCount(int current, IEnumerable<BattlePointUI> battlePointUIs)
    {
        foreach (BattlePointUI battlePointUI in this.battlePointUIs)
        {
            PoolManager.Instance.PushObj(battlePointUI.gameObject);
        }
        this.battlePointUIs.Clear();

        this.battlePointUIs.AddRange(battlePointUIs);
        TriggerDataChanged("battlePointCount", (current, this.battlePointUIs));
    }

    public void InitRoleStateUI(IEnumerable<RoleStateUI> roleStateUIs)
    {
        this.roleStateUIs.AddRange(roleStateUIs);
        TriggerDataChanged(nameof(this.roleStateUIs), roleStateUIs);
    }
}
