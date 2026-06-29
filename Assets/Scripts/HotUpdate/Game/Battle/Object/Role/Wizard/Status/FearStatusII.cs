using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Status
{
    /// <summary>
    /// 恐惧II
    /// </summary>
    [StatusTypeId(211)]
    public class FearStatusII : StatusBase
    {
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
