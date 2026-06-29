using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Status
{
    /// <summary>
    /// 恐惧
    /// </summary>
    [StatusTypeId(201)]
    public class FearStatus : StatusBase
    {
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
