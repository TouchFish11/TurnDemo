using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Game.Battle.Status;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Status
{
    /// <summary>
    /// 庇佑II
    /// </summary>
    [StatusTypeId(111)]
    public class ProtectStatusII : Battle.Status.Status
    {
        protected override void OnAdd()
        {
            Owner.TakeSheild(250);
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
