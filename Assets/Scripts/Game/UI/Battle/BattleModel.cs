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

    public void UpdateAcitonbar(IEnumerable<ActionGridUI> actionGridUIs)
    {
        actions.Clear();
        actions.AddRange(actionGridUIs);

        TriggerDataChanged(nameof(actions), actions);
    }


}
