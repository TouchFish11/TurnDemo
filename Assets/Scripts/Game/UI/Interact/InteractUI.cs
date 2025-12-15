using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ½»»¥UI
/// </summary>
public class InteractUI : UIBehaviour
{
    protected UIComponentBinder uIComponentBinder;

    protected override void Awake()
    {
        uIComponentBinder = new UIComponentBinder(this);
        //uIComponentBinder.Bind();

        uIComponentBinder.OnButtonClick += UIComponentBinder_OnButtonClick;
    }

    private void UIComponentBinder_OnButtonClick(string arg0)
    {
        LogManager.Log($"°´Å¥µã»÷");
    }

    public void Init(string text)
    {
        uIComponentBinder.GetControl<TextMeshProUGUI>("txtInteractTip").text = text;
    }

}
