using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Status
{
    /// <summary>
    /// 渊涌
    /// </summary>
    [StatusTypeId(1051)]
    public class SurgingFlowStatus : StatusBase
    {
        protected override void OnAdd()
        {
            bonusData.AtkBuildBonus += 50;
        }

        protected override void OnRemove()
        {
            bonusData.AtkBuildBonus -= 50;
        }
    }
}
