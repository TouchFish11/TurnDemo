using Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 全局消息界面
/// </summary>
public class GlobalMessageView : UIView
{
    [Inject] private ScrollRect svMsg;

    public Transform MessageContainer => svMsg.content;

    [System.Obsolete]
    public override void UpdateView(string key, object value)
    {

    }
}
