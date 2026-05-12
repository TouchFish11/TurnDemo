using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Game.Battle.Status;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Status
{
    /// <summary>
    /// 恐惧
    /// </summary>
    [StatusTypeId(201)]
    public class FearStatus : Battle.Status.Status
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
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
}
