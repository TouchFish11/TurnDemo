using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;

namespace GameHotUpdate.Battle.Status.Statuses
{
    /// <summary>
    /// ����Ч��
    /// </summary>
    [StatusTypeId(10001)]
    public class ProtectStatus : Status
    {
        protected override void OnAdd()
        {

        }

        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            // ����غ���
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
}
