using Framework;
using System.Collections;
using System.Collections.Generic;
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
        uIComponentBinder.Bind();
    }

    public void Init(string text)
    {
        uIComponentBinder.GetControl<Text>("txtName").text = text;
    }

}
