using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗消息UI
/// </summary>
public class BattleMessageUI : BaseUIBehaviour
{
    private TextMeshProUGUI txtMsg;

    private Image msg;
    private Image imgIcon;

    // 透明度
    private float msgAlpha;
    private float imgIconAlpha;

    [SerializeField] private float duration;
    // 当前持续时间
    private float currentDuration;

    protected override void Awake()
    {
        base.Awake();

        txtMsg = binder.GetControl<TextMeshProUGUI>(nameof(txtMsg));
        msg = binder.GetControl<Image>(nameof(msg));
        imgIcon = binder.GetControl<Image>(nameof(imgIcon));

        msgAlpha = msg.color.a;
        imgIconAlpha = imgIcon.color.a;

        ServiceLocator.Instance.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    protected override void OnEnable()
    {
        currentDuration = 0;
    }

    public void InitMessage(Color color, string msg)
    {
        this.msg.color = new Color(color.r, color.g, color.b, msgAlpha);
        this.imgIcon.color = new Color(color.r, color.g, color.b, imgIconAlpha);
        txtMsg.text = msg;
    }

    private void OnUpdate()
    {
        if (!this.gameObject.activeSelf)
        {
            return;
        }

        currentDuration += Time.deltaTime;
        if (currentDuration >= duration)
        {
            PoolManager.Instance.PushObj(this.gameObject);
        }
    }
}
