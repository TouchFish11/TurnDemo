using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 消息UI
/// </summary>
public class MessageUI : BaseUIBehaviour
{
    private TextMeshProUGUI txtMsg;

    // 持续时间
    [SerializeField] private float duration;
    // 当前时间
    private float currentDuration;

    protected override void Awake()
    {
        base.Awake();

        txtMsg = binder.GetControl<TextMeshProUGUI>(nameof(txtMsg));
    }

    protected override void OnEnable()
    {
        currentDuration = 0;
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化消息
    /// </summary>
    /// <param name="msg"></param>
    public void InitMessage(string msg)
    {
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

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
