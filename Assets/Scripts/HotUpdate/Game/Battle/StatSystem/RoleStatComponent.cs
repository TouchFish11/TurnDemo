using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.StatSystem.Providers;
using HotUpdate.Game.Battle.StatSystem.Resources;

namespace HotUpdate.Game.Battle.StatSystem
{
    /// <summary>
    /// 角色属性组件
    /// </summary>
    [ComponentId]
    public class RoleStatComponent : StatsComponent
    {
        public IResource UltimateResource => ((RoleStatSet)StatsComponentCore.StatSet).UltimateResource;

        protected override void OnPreInitStats()
        {
            var roleInfo = ((RoleObject)BattleEntity).RoleInfo;
            StatsComponentCore.StatSet = new RoleStatSet();
            StatsComponentCore.StatSetProvider = new RoleStatConfigProvider(roleInfo);
            ((RoleStatSet)StatsComponentCore.StatSet).UltimateResource = new EnergyResource(EResourceStatType.Energy, roleInfo.f_maxEnergy, 0);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="amount"></param>
        public void GainResource(float amount)
        {
            UltimateResource.Gain(amount);
            BattleEntity.Context.EventBus.TriggerEvent(new ResourceChangedEvent(BattleEntity.Context, BattleEntity, UltimateResource.CurrentValue, UltimateResource.MaxValue));
        }

        /// <summary>
        /// 消耗
        /// </summary>
        /// <param name="amount"></param>
        public void ConsumeResource(float amount)
        {
            UltimateResource.Consume(amount);
            BattleEntity.Context.EventBus.TriggerEvent(new ResourceChangedEvent(BattleEntity.Context, BattleEntity, UltimateResource.CurrentValue, UltimateResource.MaxValue));
        }

        /// <summary>
        /// 消耗所有
        /// </summary>
        public void ConsumeAllResource()
        {
            ConsumeResource(UltimateResource.CurrentValue);
            BattleEntity.Context.EventBus.TriggerEvent(new ResourceChangedEvent(BattleEntity.Context, BattleEntity, UltimateResource.CurrentValue, UltimateResource.MaxValue));
        }
        
        protected override void OnBattleDestroy()
        {
            
        }
    }
}
