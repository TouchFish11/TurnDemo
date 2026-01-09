using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态基类
/// </summary>
public abstract class Status : IStatus, IPoolData
{
    private bool _isValid;

    public StatusInfo StatusInfo { get; protected set; }

    public IBattleEntityObject Sourcer { get; private set; }

    public IBattleEntityObject Owner { get; private set; }

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

    public void InitStatus(IBattleEntityObject sorucer, IBattleEntityObject owner, StatusInfo statusInfo)
    {
        StatusInfo = statusInfo;
        Sourcer = sorucer;
        Owner = owner;
        IsValid = true;
    }

    /// <summary>
    /// 执行添加逻辑
    /// 当IsValid为true时将被调用
    /// </summary>
    protected abstract void OnAdd();

    /// <summary>
    /// 执行移除逻辑
    /// 当IsValid为false时将被调用
    /// </summary>
    protected abstract void OnRemove();

    public virtual void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
    {

    }

    public virtual void OnTurnEnd(IBattleEntityObject owner, IBattleContext context)
    {

    }

    public void ResetData()
    {
        _isValid = false;
        StatusInfo = null;
        Sourcer = null;
        Owner = null;
    }
}
