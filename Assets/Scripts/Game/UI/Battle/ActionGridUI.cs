using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 行动格子UI
/// </summary>
public class ActionGridUI : BaseUIBehaviour
{
    private Image imgIcon;
    private TextMeshProUGUI txtActionValue;

    protected override void Awake()
    {
        base.Awake();
        imgIcon = binder.GetControl<Image>(nameof(imgIcon));
        txtActionValue = binder.GetControl<TextMeshProUGUI>(nameof(txtActionValue));
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="actionValue"></param>
    public void Init(Sprite icon, int actionValue)
    {
        imgIcon.sprite = icon;
        txtActionValue.text = actionValue.ToString();
    }
}


