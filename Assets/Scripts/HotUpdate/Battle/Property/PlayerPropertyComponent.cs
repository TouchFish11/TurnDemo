using Core.Components;
using HotUpdate.Battle.Event.General;
using HotUpdate.Battle.Object;

namespace HotUpdate.Battle.Property
{
    /// <summary>
    /// 角色属性组件
    /// </summary>
    [ComponentId(typeof(PlayerPropertyComponent))]
    public class PlayerPropertyComponent : PropertyComponent
    {
        protected RoleProperty RoleProperty => battleProperty as RoleProperty;

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            battleProperty = new RoleProperty();
            battleProperty.InitProperty(battleEntity.BattleEntityId);
        }

        public override void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue)
        {
            base.SetPropertyValue(dynamicPropertyType, newValue);

            switch (dynamicPropertyType)
            {
                case E_DynamicPropertyType.CurrentEnergy:
                    int currentEnergyDelta = RoleProperty.CurrentEnergy - newValue;
                    RoleProperty.CurrentEnergy = newValue;
                    battleContext.GetEventBus().TriggerEvent(new EnergyChangedEvent(battleContext, BattleEntity, RoleProperty.CurrentEnergy, RoleProperty.BaseEnergy, currentEnergyDelta));
                    break;
            }
        }

        public override int GetPropertyValue(E_DynamicPropertyType dynamicPropertyType)
        {
            switch (dynamicPropertyType)
            {
                case E_DynamicPropertyType.BaseEnergy:
                    return RoleProperty.BaseEnergy;
                case E_DynamicPropertyType.CurrentEnergy:
                    return RoleProperty.CurrentEnergy;
            }
            return base.GetPropertyValue(dynamicPropertyType);
        }
    }
}
