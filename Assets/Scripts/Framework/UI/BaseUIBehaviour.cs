using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 基础UI行为类
/// </summary>
public abstract class BaseUIBehaviour : UIBehaviour
{
    protected UIComponentBinder binder;

    protected override void Awake()
    {
        binder = new UIComponentBinder(this);
        binder.OnButtonClick += OnButtonClick;
        binder.OnSliderValueChanged += OnSliderValueChanged;
        binder.OnInputFieldValueChanged += OnInputFieldValueChanged;
        binder.OnToggleValueChanged += OnToggleValueChanged;
    }

    protected virtual void OnButtonClick(string btnName) { }

    protected virtual void OnSliderValueChanged(string sliderName, float value) { }

    protected virtual void OnInputFieldValueChanged(string inputFieldName, string value) { }

    protected virtual void OnToggleValueChanged(string togName, bool isOn) { }

    protected override void OnDestroy()
    {
        binder.Clear();
    }
}
