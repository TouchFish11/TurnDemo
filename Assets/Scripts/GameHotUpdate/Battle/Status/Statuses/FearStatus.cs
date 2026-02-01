using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;

namespace GameHotUpdate.Battle.Status.Statuses
{
    /// <summary>
    /// �־�
    /// </summary>
    [StatusTypeId(20001)]
    public class FearStatus : Status
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            // ����غ���
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
