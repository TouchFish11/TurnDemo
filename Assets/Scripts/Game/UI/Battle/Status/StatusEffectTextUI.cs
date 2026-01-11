using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态效果文本
/// </summary>
public class StatusEffectTextUI : BaseUIBehaviour
{
    private Image imgIcon;
    private TextMeshProUGUI txtBuffName;

    private Transform mover;

    // 上移速度
    [SerializeField] private float upMoveSpeed = 2.5f;
    // 销毁时间
    [SerializeField] private float destroyTime = 0.85f;

    // 起始位置
    private Vector3 originMoverPos;
    // 当前时间
    private float currentTime;

protected override void Awake()
    {
        base.Awake();

        imgIcon = binder.GetControl<Image>(nameof(imgIcon));
        txtBuffName = binder.GetControl<TextMeshProUGUI>(nameof(txtBuffName));
        mover = this.transform.Find(nameof(mover));

        originMoverPos = mover.localPosition;
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpadte);
    }

    protected override void OnEnable()
    {
        mover.localPosition = originMoverPos;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="buffName"></param>
    public void InitText(Sprite icon, string buffName)
    {
        imgIcon.sprite = icon;
        txtBuffName.text = buffName;
    }

    private void OnUpadte()
    {
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
        //文本运动
        this.mover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
    }
}
