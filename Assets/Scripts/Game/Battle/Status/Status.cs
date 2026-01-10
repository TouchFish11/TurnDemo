using Framework;
using Game.Battle;

/// <summary>
/// 状态基类
/// </summary>
public abstract class Status : IStatus, IPoolData
{
    // 是否有效
    private bool _isValid;
    // 加成数据
    protected StatusBonusData bonusData;

    public StatusProperty StatusProperty { get; protected set; }

    public IBattleEntityObject Sourcer { get; private set; }

    public IBattleEntityObject Owner { get; private set; }

    public StatusBonusData BonusData => bonusData;

    public bool IsValid
    {
        get => _isValid;
        set
        {
            _isValid = value;
            if (value)
            {
                OnAdd();
            }
            else
            {
                OnRemove();
            }
        }
    }

    public void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, int statusId)
    {
        StatusProperty = new StatusProperty(statusId);
        bonusData = new StatusBonusData();
        Sourcer = sorucer;
        Owner = owner;
    }

    public void ChangePine(int deltaPine)
    {
        // 更新数据
        StatusProperty.SetCurrentPine(StatusProperty.CurrentPine + deltaPine);
        // 更新效果
        OnPineChanged();
    }

    public virtual void TurnStart(IBattleEntityObject owner, IBattleContext context)
    {
        OnTurnStart(owner, context);

        // 判断剩余回合数、层数是否有效
        if (StatusProperty.RemainingRound <= 0 || StatusProperty.CurrentPine <= 0)
        {
            IsValid = false;
        }
    }

    public virtual void TurnEnd(IBattleEntityObject owner, IBattleContext context)
    {
        OnTurnEnd(owner, context);
    }

    /// <summary>
    /// 执行添加逻辑
    /// 当IsValid为true时将被调用
    /// </summary>
    protected virtual void OnAdd() { }

    /// <summary>
    /// 层数变化执行
    /// </summary>
    protected virtual void OnPineChanged() { }

    /// <summary>
    /// 执行移除逻辑
    /// 当IsValid为false时将被调用
    /// </summary>
    protected virtual void OnRemove() { }

    /// <summary>
    /// 回合开始逻辑
    /// 结算回合、层数。不同状态有不同的结算规则，需自定义
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="context"></param>
    protected abstract void OnTurnStart(IBattleEntityObject owner, IBattleContext context);

    /// <summary>
    /// 回合结束逻辑
    /// 须在回合结束时处理的特殊逻辑，可在这里自定义
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="context"></param>
    protected virtual void OnTurnEnd(IBattleEntityObject owner, IBattleContext context) { }

    public void ResetData()
    {
        _isValid = false;
        StatusProperty = null;
        Sourcer = null;
        Owner = null;
    }
}
