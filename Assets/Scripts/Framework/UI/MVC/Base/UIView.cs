using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIView : BaseUIBehaviour, IUIView
{
    // 画布组
    protected CanvasGroup canvasGroup;
    // 透明度变化率
    protected float alphaSpeed = 1f;
    // 是否隐藏
    private bool _isHide;
    // 隐藏回调
    private UnityAction _hideCallBack;

    protected override void Awake()
    {
        base.Awake();
        this.canvasGroup = this.GetComponent<CanvasGroup>();
    }

    protected virtual void Update()
    {
        // 逐渐隐藏
        if (this._isHide && this.canvasGroup.alpha > 0)
        {
            this.canvasGroup.alpha -= Time.unscaledDeltaTime * this.alphaSpeed;
            if (this.canvasGroup.alpha < 0)
            {
                this.canvasGroup.alpha = 0;
                this._hideCallBack?.Invoke();
                this._hideCallBack = null;
            }
        }
        // 逐渐显示
        else if (!this._isHide && this.canvasGroup.alpha < 1)
        {
            this.canvasGroup.alpha += Time.unscaledDeltaTime * this.alphaSpeed;
            if (this.canvasGroup.alpha > 1)
            {
                this.canvasGroup.alpha = 1;
            }
        }
    }

    /// <summary>
    /// 显示
    /// </summary>
    public virtual void Show()
    {
        this._isHide = false;
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    /// <param name="hideCallBack">隐藏结束回调</param>
    public virtual void Hide(UnityAction hideCallBack = null)
    {
        this._isHide = true;
        this._hideCallBack = hideCallBack;
    }

    /// <summary>
    /// 更新界面（子类实现，接收 Controller 指令）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public abstract void UpdateView(string key, object value);

    /// <summary>
    /// 获取绑定器
    /// </summary>
    /// <returns></returns>
    public UIComponentBinder GetBinder()
    {
        return binder;
    }
}
