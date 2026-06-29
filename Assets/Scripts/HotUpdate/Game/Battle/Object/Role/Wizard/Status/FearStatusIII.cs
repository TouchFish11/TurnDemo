using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Status
{
    /// <summary>
    /// 恐惧III
    /// </summary>
    [StatusTypeId(221)]
    public class FearStatusIII : StatusBase
    {
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
