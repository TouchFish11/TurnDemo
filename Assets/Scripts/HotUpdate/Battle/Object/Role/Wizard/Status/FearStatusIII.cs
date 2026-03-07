using HotUpdate.Battle.Status;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Object.Role.Wizard.Status
{
    /// <summary>
    /// 恐惧III
    /// </summary>
    [StatusTypeId(221)]
    public class FearStatusIII : Battle.Status.Status
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            // ����غ���
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
        }

        protected override void OnPineChanged()
        {
            bonusData.AtkBuildBonus -= 40;
        }

        protected override void OnRemove()
        {
            bonusData.AtkBuildBonus += 40;
        }
    }
}
