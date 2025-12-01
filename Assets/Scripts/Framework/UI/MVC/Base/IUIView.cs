using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IUIView
{
    /// <summary>
    /// 显示
    /// </summary>
    void Show();

    /// <summary>
    /// 隐藏
    /// </summary>
    /// <param name="hideCallBack"></param>
    void Hide(UnityAction hideCallBack = null);

    /// <summary>
    /// 更新视图
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void UpdateView(string key, object value);
}
