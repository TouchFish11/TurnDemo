using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIModel
{
    /// <summary>
    /// 数据变更事件（给 Controller 订阅）
    /// </summary>
    public event Action<string, object> OnDataChanged;
}
