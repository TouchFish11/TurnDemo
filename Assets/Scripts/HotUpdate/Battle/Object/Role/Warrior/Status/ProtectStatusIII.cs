using HotUpdate.Battle.Context;
using HotUpdate.Battle.Status;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Object.Role.Warrior.Status
{
    /// <summary>
    /// 庇佑III
    /// </summary>
    [StatusTypeId(121)]
    public class ProtectStatusIII : Battle.Status.Status
    {
        protected override void OnAdd()
        {
            Owner.TakeSheild(350);
        }

        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
        }

        protected override void OnPineChanged()
        {
            bonusData.DefBuildBonus += 50;
        }

        protected override void OnRemove()
        {
            bonusData.DefBuildBonus -= 50;
        }
    }
}
