using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;

namespace GameHotUpdate.Battle.Object.Role.Warrior.Status
{
    /// <summary>
    /// 庇佑
    /// </summary>
    [StatusTypeId(101)]
    public class ProtectStatus : Battle.Status.Status
    {
        protected override void OnAdd()
        {
            bonusData.DefBuildBonus += 20;
            Owner.TakeSheild(150);
        }

        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
        }

        protected override void OnPineChanged()
        {

        }

        protected override void OnRemove()
        {
            bonusData.DefBuildBonus -= 20;
        }
    }
}
