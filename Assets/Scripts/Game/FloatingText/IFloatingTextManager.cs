using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 浮动文本管理器接口
/// </summary>
public interface IFloatingTextManager
{
    /// <summary>
    /// 初始化
    /// </summary>
    void Init();

    /// <summary>
    /// 清空缓存
    /// </summary>
    void ClearCache();
}
