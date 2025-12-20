using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillKeyUI : BaseUIBehaviour
{
    private Toggle togSkillKeyUI;
    private TextMeshProUGUI txtSkillTip;
    private readonly Vector3 SelectedScale = Vector3.one * 1.5f;

    protected override void Awake()
    {
        base.Awake();
        togSkillKeyUI = binder.GetControl<Toggle>(this.gameObject.name);
        txtSkillTip = binder.GetControl<TextMeshProUGUI>(nameof(txtSkillTip));
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="skillTip"></param>
    public void Init(string skillTip)
    {
        txtSkillTip.text = skillTip;
    }

    /// <summary>
    /// 默认选择
    /// </summary>
    public void DefaultSelect()
    {
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
            this.transform.localScale = SelectedScale;
        }
        else
        {
            this.transform.localScale = Vector3.one;
        }
    }
}
