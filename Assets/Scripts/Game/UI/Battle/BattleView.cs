using Game.Battle;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : UIView
{
    private ScrollRect svActionbar;
    private ScrollRect svPoint;

    private TextMeshProUGUI txtCount;

    private Transform operatorArea;
    private Transform playerArea;

    /// <summary>
    /// 技能键组
    /// </summary>
    public ToggleGroup SkillKeyGroup { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        svActionbar = binder.GetControl<ScrollRect>(nameof(svActionbar));
        svPoint = binder.GetControl<ScrollRect>(nameof(svPoint));

        txtCount = binder.GetControl<TextMeshProUGUI>(nameof(txtCount));

        operatorArea = this.transform.Find(nameof(operatorArea));
        playerArea = this.transform.Find(nameof(playerArea));

        SkillKeyGroup = binder.GetControl<ToggleGroup>(nameof(operatorArea));
    }


    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "actions":
                UpdateActionbar(value as List<ActionGridUI>);
                break;
            case "skillKeyUIs":
                UpdateOperator(value as List<SkillKeyUI>);
                break;
            case "roleStateUIs":
                InitRoleStateUI(value as List<RoleStateUI>);
                break;
            case "battlePointCount":
                (int currentBP, List<BattlePointUI> battlePointUIs) = ((int currentBP, List<BattlePointUI>))value;
                UpdateBattlePointCount(currentBP, battlePointUIs);
                break;
        }
    }

    /// <summary>
    /// 更新行动栏
    /// </summary>
    private void UpdateActionbar(List<ActionGridUI> actionGridUIs)
    {
        svActionbar.content.DetachChildren();
        foreach (ActionGridUI actionGridUI in actionGridUIs)
        {
            actionGridUI.transform.SetParent(svActionbar.content, false);
        }
    }

    public void UpdateOperator(List<SkillKeyUI> skillKeyUIs)
    {
        operatorArea.DetachChildren();
        foreach (SkillKeyUI skillKeyUI in skillKeyUIs)
        {
            skillKeyUI.transform.SetParent(operatorArea, false);
        }
    }

    public void InitRoleStateUI(IEnumerable<RoleStateUI> roleStateUIs)
    {
        foreach (RoleStateUI roleStateUI in roleStateUIs)
        {
            roleStateUI.transform.SetParent(playerArea, false);
        }
    }

    public void UpdateBattlePointCount(int current, List<BattlePointUI> battlePointUIs)
    {
        txtCount.text = current.ToString();
        foreach (BattlePointUI battlePointUI in battlePointUIs)
        {
            battlePointUI.transform.SetParent(svPoint.content, false);
        }
    }
}
