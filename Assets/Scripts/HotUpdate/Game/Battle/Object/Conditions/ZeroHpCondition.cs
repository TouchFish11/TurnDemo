using HotUpdate.Game.Battle.Property;

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
