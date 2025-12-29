using Framework;
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
    private TextMeshProUGUI txtDmg;

    private TextMeshProUGUI txtActingTip;
    private Image imgActingIcon;

    private Transform operatorArea;
    private Transform playerArea;
    private Transform selectMarkerArea;

    private GameObject totalDmgArea;
    private ActingTipUI actingTipUI;

    /// <summary>
    /// 技能键组
    /// </summary>
    public ToggleGroup SkillKeyGroup { get; private set; }

    public Transform SelectMarkerArea => selectMarkerArea;

    protected override void Awake()
    {
        base.Awake();

        svActionbar = binder.GetControl<ScrollRect>(nameof(svActionbar));
        svPoint = binder.GetControl<ScrollRect>(nameof(svPoint));

        txtCount = binder.GetControl<TextMeshProUGUI>(nameof(txtCount));
        txtDmg = binder.GetControl<TextMeshProUGUI>(nameof(txtDmg));
        txtActingTip = binder.GetControl<TextMeshProUGUI>(nameof(txtActingTip));
        imgActingIcon = binder.GetControl<Image>(nameof(imgActingIcon));

        operatorArea = this.transform.Find(nameof(operatorArea));
        playerArea = this.transform.Find(nameof(playerArea));
        selectMarkerArea = this.transform.Find(nameof(selectMarkerArea));
        totalDmgArea = this.transform.Find(nameof(totalDmgArea)).gameObject;
        totalDmgArea.SetActive(false);

        actingTipUI = this.GetComponentInChildren<ActingTipUI>();
        actingTipUI.Init(imgActingIcon, txtActingTip);
        actingTipUI.gameObject.SetActive(false);

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
            case "selectMarkerUIs":
                UpdateSelectMarker(value as List<SelectMarkerUI>);
                break;
            case "currentCalcDamage":
                (bool isShow, long dmg) = ((bool, long))value;
                totalDmgArea.SetActive(isShow);
                if (isShow)
                {
                    txtDmg.text = dmg.ToString();
                }
                break;
            case "activeActTip":
                (bool isActive, bool isMonster) = ((bool, bool))value;
                actingTipUI.gameObject.SetActive(isActive);
                if (isActive)
                {
                    actingTipUI.UpdateTipText(isMonster);
                }
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

    public void UpdateSelectMarker(List<SelectMarkerUI> selectMarkerUIs)
    {
        foreach (SelectMarkerUI selectMarkerUI in selectMarkerUIs)
        {
            selectMarkerUI.transform.SetParent(selectMarkerArea, false);
        }
    }
}
