using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ½»»¥UI
/// </summary>
public class InteractUI : UIBehaviour
{
    protected UIComponentBinder uIComponentBinder;

    protected override void Awake()
    {
        uIComponentBinder = new UIComponentBinder(this);
    }
}
