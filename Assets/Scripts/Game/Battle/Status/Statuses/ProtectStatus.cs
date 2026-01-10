using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 庇佑效果
/// </summary>
public class ProtectStatus : Status
{
    protected override void OnAdd()
    {

    }

    protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
    {
        // 结算回合数
        StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
    }

    protected override void OnPineChanged()
    {
        bonusData.DefBuildBonus += 20;
    }

    protected override void OnRemove()
    {
        bonusData.DefBuildBonus -= 20;
    }
}
