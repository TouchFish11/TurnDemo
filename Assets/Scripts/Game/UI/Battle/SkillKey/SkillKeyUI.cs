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
    //// 角色信息
    //private RoleInfo roleInfo;
    // 能否触发技能
    private E_TriggerPhase triggerPhase = E_TriggerPhase.NonSeleceted;
    // 战斗上下文接口
    private IBattleContext battleContext;
    // 战斗实体接口
    private IBattleEntityObject battleEntity;
    // 当前技能类型
    private E_SkillType _SkillType;

    protected override void Awake()
    {
        base.Awake();
        togSkillKeyUI = binder.GetControl<Toggle>(this.gameObject.name);
        txtSkillTip = binder.GetControl<TextMeshProUGUI>(nameof(txtSkillTip));
        UIManager.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
        battleContext = ServiceLocator.Instance.Get<IBattleManager>().GetContext();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="skillInfo"></param>
    public void Init(SkillInfo skillInfo, ToggleGroup group, IBattleEntityObject battleEntity)
    {
        this.skillId = skillInfo.f_id;
        togSkillKeyUI.group = group;
        this.battleEntity = battleEntity;
        txtSkillTip.text = skillInfo.f_skillRangeType.ToSkillRangeTypeText();

        // TODO：暂时直接判断，后续抽象
        _SkillType = (E_SkillType)skillInfo.f_skillRangeType;
        if (_SkillType == E_SkillType.NormalAttack || _SkillType == E_SkillType.UltimateSkill)
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
        // 激活目标标记
        TargetSelectManager.Instance.ActiveSelectTarget();
        togSkillKeyUI.isOn = true;
    }

    protected override void OnToggleValueChanged(string togName, bool isOn)
    {
        OnSelected(isOn);
    }

    private void OnSelected(bool isOn)
    {
        if (isOn)
        {
            if (triggerPhase == E_TriggerPhase.Selected)
            {
                triggerPhase = E_TriggerPhase.Trigger;
            }
            else
            {
                // 选中：放大+标记为Selected
                this.transform.localScale = SelectedScale;
                triggerPhase = E_TriggerPhase.Selected;
                battleContext.GetEventBus().TriggerEvent(new SelectSkillEvent(battleContext, skillId, battleEntity));
            }
        }
        else
        {
            // 取消选中：缩放到1倍+标记为NonSeleceted
            this.transform.localScale = Vector3.one;
            triggerPhase = E_TriggerPhase.NonSeleceted;
        }
    }

    private void OnClick(BaseEventData baseEventData)
    {
        if (triggerPhase == E_TriggerPhase.Trigger && _SkillType != E_SkillType.UltimateSkill)
        {
            triggerPhase = E_TriggerPhase.Selected;
            // 执行触发技能事件
            battleContext.GetEventBus().TriggerEvent(new PlayerTriggerSkillEvent(battleContext, skillId, battleEntity));
        }
        else
        {
            // 释放终结技
            // TODO：暂时直接调用，后续优化
            battleEntity.GetComponent<PlayerSkillComponent>().ReleaseUltimate();
        }
    }

    /// <summary>
    /// 重置状态
    /// </summary>
    private void ResetState()
    {
        togSkillKeyUI.group = null;
        // 重置Toggle状态
        togSkillKeyUI.isOn = false;
        // 重置逻辑状态（和Toggle强绑定）
        triggerPhase = E_TriggerPhase.NonSeleceted;
        // 重置视觉状态
        this.transform.localScale = Vector3.one;
        battleEntity = null;
    }

    protected override void OnDisable()
    {
        ResetState();
    }
}
