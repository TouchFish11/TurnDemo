using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Status;
using GameHotUpdate.Battle.Property;

namespace GameHotUpdate.Battle.Object.Role.Priest.Status
{
    /// <summary>
    /// 生机III
    /// </summary>
    [StatusTypeId(321)]
    public class RejuvenationStatusIII : Battle.Status.Status
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
            var newHp = owner.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentHp) + 60;
            owner.GetComponent<PropertyComponent>().SetPropertyValue(E_DynamicPropertyType.CurrentHp, newHp);
        }
    }
}
