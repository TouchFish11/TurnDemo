using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Status
{
    /// <summary>
    /// 庇佑III
    /// </summary>
    [StatusTypeId(121)]
    public class ProtectStatusIII : StatusBase
    {
        protected override void OnAdd()
        {
            Owner.TakeSheild(350);
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
