using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI数据模型
/// </summary>
public abstract class UIModel : IUIModel
{
    /// <summary>
    /// 数据变更事件（给 Controller 订阅）
    /// </summary>
    public event Action<string, object> OnDataChanged;

    /// <summary>
    /// 触发数据变更（子类调用）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    protected void TriggerDataChanged(string key, object value)
    {
        OnDataChanged?.Invoke(key, value);
    }

    public virtual void ClearData()
    {

    }
}
