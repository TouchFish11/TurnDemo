using HotUpdate.Battle.Context;
using HotUpdate.Battle.Status;

namespace HotUpdate.Battle.Object.Monster.AbyssalMage.Status
{
    /// <summary>
    /// 渊涌
    /// </summary>
    [StatusTypeId(1051)]
    public class SurgingFlowStatus : Battle.Status.Status
    {
        protected override void OnAdd()
        {
            bonusData.AtkBuildBonus += 50;
        }

        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
        }

        protected override void OnRemove()
        {
            bonusData.AtkBuildBonus -= 50;
        }
    }
}
