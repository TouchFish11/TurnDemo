using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTargetSelect : ITargetSelect
{
    // 目标列表（包含主目标）
    private List<IBattleEntityObject> _selectedTargets = new List<IBattleEntityObject>();

    // 主目标
    private IBattleEntityObject _mainTarget;

    public IBattleEntityObject GetMainTarget()
    {
        return _mainTarget;
    }

    public List<IBattleEntityObject> GetTargets()
    {
        return _selectedTargets;
    }

    public void UpdateTargets(IBattleEntityObject mainTarget, List<IBattleEntityObject> targets)
    {
        this._mainTarget = mainTarget;
        this._selectedTargets = targets;
    }
}
