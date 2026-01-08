using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 全局消息界面
/// </summary>
public class GlobalMessageView : UIView
{
    private ScrollRect svMsg;

    public Transform MessageContainer => svMsg.content;

    protected override void Awake()
    {
        base.Awake();

        svMsg = binder.GetControl<ScrollRect>(nameof(svMsg));
    }


    public override void UpdateView(string key, object value)
    {

    }
}
