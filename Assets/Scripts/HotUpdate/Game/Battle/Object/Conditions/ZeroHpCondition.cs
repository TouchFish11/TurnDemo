using HotUpdate.Game.Battle.Property;

namespace HotUpdate.Game.Battle.Object.Conditions
{
    public class ZeroHpCondition : IDeathCondition
    {
        public bool CanDie(IBattleEntityObject battleEntity)
        {
            return battleEntity.GetComponent<StatComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentHp) <= 0;
        }
    }
}
