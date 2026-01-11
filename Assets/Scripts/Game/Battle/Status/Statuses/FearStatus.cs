using Game.Battle;

/// <summary>
/// 恐惧
/// </summary>
[StatusTypeId(20001)]
public class FearStatus : Status
{
    protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
    {
        // 结算回合数
        StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
    }

    protected override void OnPineChanged()
    {
        bonusData.AtkBuildBonus -= 20;
    }

    protected override void OnRemove()
    {
        bonusData.AtkBuildBonus += 20;
    }
}
