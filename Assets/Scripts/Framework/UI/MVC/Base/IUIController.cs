using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIController
{
    /// <summary>
    /// 绑定视图事件
    /// </summary>
    void BindViewEvents();

    /// <summary>
    /// 绑定模型数据事件
    /// </summary>
    void BindModelEvents();

    /// <summary>
    /// 模型数据变化处理
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void OnHandleModelDataChanged(string key, object value);
}
