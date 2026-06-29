using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Status
{
    /// <summary>
    /// 庇佑II
    /// </summary>
    [StatusTypeId(111)]
    public class ProtectStatusII : StatusBase
    {
        protected override void OnAdd()
        {
            Owner.TakeSheild(250);
        }
        
        protected override void OnPineChanged()
        {
            bonusData.DefBuildBonus += 40;
        }

        protected override void OnRemove()
        {
            bonusData.DefBuildBonus -= 40;
        }
    }
}
