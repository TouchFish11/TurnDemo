using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗界面数据
/// </summary>
public class BattleModel : UIModel
{
    // 行动条格子UI列表
    private readonly List<ActionGridUI> actions = new List<ActionGridUI>();
    private readonly List<SkillKeyUI> skillKeyUIs = new List<SkillKeyUI>();


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
}
