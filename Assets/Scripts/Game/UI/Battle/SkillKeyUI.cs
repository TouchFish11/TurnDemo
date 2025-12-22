using Framework;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 技能按键UI
/// </summary>
public class SkillKeyUI : BaseUIBehaviour
{
    /// <summary>
    /// 按键触发阶段
    /// </summary>
    private enum E_TriggerPhase
    {
        /// <summary>
        /// 未选中
        /// </summary>
        NonSeleceted,
        /// <summary>
        /// 已选中
        /// </summary>
        Selected,
        /// <summary>
        /// 触发
        /// </summary>
        Trigger,
    }

    private Toggle togSkillKeyUI;
    private TextMeshProUGUI txtSkillTip;
    // 选择时的缩放比例
    private readonly Vector3 SelectedScale = Vector3.one * 1.3f;
    // 技能ID
    private int skillId;
    // 角色信息
    private RoleInfo roleInfo;
    // 能否触发技能
    private E_TriggerPhase triggerPhase = E_TriggerPhase.NonSeleceted;

    /// <summary>
    /// 触发技能事件
    /// </summary>
    public event Action<int> OnTriggerSkill;

    protected override void Awake()
    {
        base.Awake();
        togSkillKeyUI = binder.GetControl<Toggle>(this.gameObject.name);
        txtSkillTip = binder.GetControl<TextMeshProUGUI>(nameof(txtSkillTip));
        UIManager.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="skillInfo"></param>
    public void Init(SkillInfo skillInfo, RoleInfo roleInfo, ToggleGroup group)
    {
        this.skillId = skillInfo.f_id;
        this.roleInfo = roleInfo;
        togSkillKeyUI.group = group;
        txtSkillTip.text = skillInfo.f_skillRangeType.ToSkillRangeTypeText();

        if(skillInfo.f_skillRangeType.ToSkillType() == E_SkillType.NormalAttack)
        {
            // 自身技能默认选中
            DefaultSelect();
        }
    }

    /// <summary>
    /// 默认选择
    /// </summary>
    public void DefaultSelect()
    {
        togSkillKeyUI.isOn = true;
        triggerPhase = E_TriggerPhase.Selected;
    }

    protected override void OnToggleValueChanged(string togName, bool isOn)
    {
        OnSelected(isOn);
    }

    private void OnSelected(bool isOn)
    {
        if (isOn)
        {
            this.transform.localScale = SelectedScale;
            if (triggerPhase == E_TriggerPhase.Selected)
            {
                triggerPhase = E_TriggerPhase.Trigger;
            }
            else
            {
                triggerPhase = E_TriggerPhase.Selected;
            }
        }
        else
        {
            this.transform.localScale = Vector3.one;
            if (triggerPhase == E_TriggerPhase.Selected)
            {
                triggerPhase = E_TriggerPhase.NonSeleceted;
            }
        }
    }

    private void OnClick(BaseEventData baseEventData)
    {
        if (triggerPhase == E_TriggerPhase.Trigger)
        {
            // 执行触发技能事件
            OnTriggerSkill?.Invoke(skillId);
            triggerPhase = E_TriggerPhase.Selected;
        }
    }
}
