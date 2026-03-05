using HotUpdate.Battle.Context;
using HotUpdate.Battle.Status;

namespace HotUpdate.Battle.Object.Role.Wizard.Status
{
    /// <summary>
    /// 恐惧II
    /// </summary>
    [StatusTypeId(211)]
    public class FearStatusII : Battle.Status.Status
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            // ����غ���
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
        }

        protected override void OnPineChanged()
        {
            bonusData.AtkBuildBonus -= 30;
        }

        protected override void OnRemove()
        {
            bonusData.AtkBuildBonus += 30;
        }
    }
}
