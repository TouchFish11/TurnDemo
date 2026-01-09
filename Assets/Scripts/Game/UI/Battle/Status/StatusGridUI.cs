using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态格子UI
/// </summary>
public class StatusGridUI : BaseUIBehaviour
{
    private IStatus status;

    protected override void Awake()
    {
        base.Awake();


    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="status"></param>
    public void Init(IStatus status)
    {
        this.status = status;
    }

    public int GetStatusId() => status.StatusProperty.StatusInfo.f_id;

    public bool IsValid => status.IsValid;
}
