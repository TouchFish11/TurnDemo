using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鼠标管理器接口
/// </summary>
public interface IMouseManager
{
    bool Visible { get; }

    void ReleaseMouseVisible(string sorce);
    void RequestMouseVisible(string sorce);
}
