using HotUpdate.Game.Battle.StatSystem;

namespace HotUpdate.Game.Battle.Object.Conditions
{
    public class ZeroHpCondition : IDeathCondition
    {
        public bool CanDie(IBattleEntityObject battleEntity)
        {
            return battleEntity.GetComponent<StatsComponent>().CurrentHp <= 0;
        }
    }
}
