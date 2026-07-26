using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 角色属性组件
    /// </summary>
    [ComponentId]
    public class PlayerPropertyComponent : PropertyComponent
    {
        protected RoleProperty RoleProperty => battleProperty as RoleProperty;

        protected override void OnBattleInit()
        {
            battleProperty = new RoleProperty();
            ((RoleProperty)battleProperty).InitProperty(((IPlayerObject)BattleEntity).RoleInfo);
        }

        public override void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue)
        {
            base.SetPropertyValue(dynamicPropertyType, newValue);

            switch (dynamicPropertyType)
            {
                case E_DynamicPropertyType.CurrentEnergy:
                    var currentEnergyDelta = RoleProperty.CurrentEnergy - newValue;
                    RoleProperty.CurrentEnergy = newValue;
                    Context.EventBus.TriggerEvent(new EnergyChangedEvent(Context, BattleEntity, RoleProperty.CurrentEnergy, RoleProperty.BaseEnergy, currentEnergyDelta));
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
        
        protected override void OnBattleDestroy()
        {
            
        }
    }
}
