using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Status
{
    /// <summary>
    /// 庇佑
    /// </summary>
    [StatusTypeId(101)]
    public class ProtectStatus : StatusBase
    {
        protected override void OnAdd()
        {
            bonusData.DefBuildBonus += 20;
            Owner.TakeSheild(150);
        }
        
        protected override void OnRemove()
        {
            bonusData.DefBuildBonus -= 20;
        }
    }
}
