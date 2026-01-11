using Framework;

/// <summary>
/// 状态属性
/// </summary>
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
        StatusInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<StatusInfoContainer>(E_ConfigLoadType.Editor).dataDic[statusId];
        currentPine = StatusInfo.f_startPine;
        remainingRound = StatusInfo.f_durationRound;
    }

    /// <summary>
    /// 剩余回合
    /// </summary>
    public int RemainingRound { get => remainingRound; }
    /// <summary>
    /// 当前层数
    /// </summary>
    public int CurrentPine { get => currentPine; }

    /// <summary>
    /// 设置剩余回合数
    /// </summary>
    /// <param name="remainingRound"></param>
    public void SetRemainingRound(int remainingRound)
    {
        this.remainingRound = remainingRound;
    }

    /// <summary>
    /// 设置当前层数
    /// </summary>
    /// <param name="currentPine"></param>
    public void SetCurrentPine(int currentPine)
    {
        this.currentPine = currentPine;
    }
}
