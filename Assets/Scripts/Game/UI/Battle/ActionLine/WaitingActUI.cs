using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// µÈ´ýÐÐ¶¯UI
/// </summary>
public class WaitingActUI : BaseUIBehaviour
{
    private Image imgIcon;

    protected override void Awake()
    {
        base.Awake();

        imgIcon = binder.GetControl<Image>(nameof(imgIcon));
    }

    public void Init(Sprite icon)
    {
        imgIcon.sprite = icon;
    }
}
