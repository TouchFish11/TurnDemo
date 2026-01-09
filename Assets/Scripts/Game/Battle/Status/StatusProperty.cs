using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusProperty
{
    /// <summary>
    /// 状态信息
    /// </summary>
    public StatusInfo StatusInfo { get; }

    // 动态属性
    private int remainingRound; // 剩余回合
    private int currentPine;    // 当前层数

    public StatusProperty(int statusId)
    {
        StatusInfo = ServiceLocator.Instance.Get<IBinaryDataManager>().GetConfig<StatusInfoContainer>(E_ConfigLoadType.Editor).dataDic[statusId];
        currentPine = StatusInfo.f_startPine;
        remainingRound = StatusInfo.f_durationRound;
    }

    /// <summary>
    /// 剩余回合
    /// </summary>
    public int RemainingRound { get => remainingRound; set => remainingRound = value; }
    /// <summary>
    /// 当前层数
    /// </summary>
    public int CurrentPine { get => currentPine; set => currentPine = value; }
}
