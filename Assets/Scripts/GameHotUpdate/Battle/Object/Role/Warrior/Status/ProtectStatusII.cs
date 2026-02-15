using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;

namespace GameHotUpdate.Battle.Object.Role.Warrior.Status
{
    /// <summary>
    /// 庇佑II
    /// </summary>
    [StatusTypeId(111)]
    public class ProtectStatusII : Battle.Status.Status
    {
        protected override void OnAdd()
        {

        }

        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
        }

        protected override void OnPineChanged()
        {
            bonusData.DefBuildBonus += 40;
        }

        protected override void OnRemove()
        {
            bonusData.DefBuildBonus -= 40;
        }
    }
}
