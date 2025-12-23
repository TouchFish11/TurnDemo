using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 伤害文本UI
/// </summary>
public class DamageTextUI : BaseUIBehaviour
{
    private TextMeshProUGUI txtDamageTip;
    private TextMeshProUGUI txtDamageNum;

    private Transform damageTextMover;

    //上移速度
    [SerializeField] private float upMoveSpeed = 4f;
    //销毁时间
    [SerializeField] private float destroyTime = 1.2f;
    [SerializeField] private Vector3 StartScale = Vector3.one;
    [SerializeField] private Vector3 endScale = Vector3.one * 0.5f;
    [SerializeField] private float scaleFactor = 5f;

    private float currentTime;

    protected override void Awake()
    {
        base.Awake();

        txtDamageTip = binder.GetControl<TextMeshProUGUI>(nameof(txtDamageTip));
        txtDamageNum = binder.GetControl<TextMeshProUGUI>(nameof(txtDamageNum));

        damageTextMover = this.transform.Find(nameof(damageTextMover));
    }

    protected override void OnEnable()
    {
        MonoManager.Instance.AddUpdateListener(OnUpdate);
        (damageTextMover.transform as RectTransform).anchoredPosition = Vector3.zero;
        damageTextMover.localScale = StartScale;
    }

    /// <summary>
    /// 初始化伤害文本
    /// </summary>
    /// <param name="textColor"></param>
    /// <param name="damageTypeText"></param>
    /// <param name="damage"></param>
    public void InitDamageText(Color textColor, string damageTypeText, int damage)
    {
        txtDamageTip.color = textColor;
        txtDamageNum.color = textColor;

        txtDamageTip.text = damageTypeText;
        txtDamageNum.text = damage.ToString();
    }

    private void OnUpdate()
    {
        // 文本动画
        if (!this.gameObject.activeSelf)
        {
            return;
        }

        currentTime += Time.deltaTime;
        if (currentTime >= destroyTime)
        {
            currentTime = 0;
            PoolManager.Instance.PushObj(this.gameObject);
        }

        // 文本缩放
        this.damageTextMover.localScale = Vector3.Lerp(this.damageTextMover.localScale, endScale, Time.deltaTime * scaleFactor);
        //文本运动
        this.damageTextMover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
    }

    protected override void OnDisable()
    {
        MonoManager.Instance.RemoveUpdateListener(OnUpdate);
    }
}
