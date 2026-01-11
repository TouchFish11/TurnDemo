using Game.Battle;

/// <summary>
/// 庇佑效果
/// </summary>
[StatusTypeId(10001)]
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
