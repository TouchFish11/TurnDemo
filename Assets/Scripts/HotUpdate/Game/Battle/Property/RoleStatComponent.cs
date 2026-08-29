using System;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Providers;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Resources;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 角色属性组件
    /// </summary>
    [ComponentId]
    public class RoleStatComponent : StatsComponent
    {
        public IResource UltimateResource => ((RoleStatSet)StatsComponentCore.StatSet).UltimateResource;
                
        /// <summary>
        /// 终结技资源事件事件（curt, max）
        /// </summary>
        public event Action<float, float> OnResourceChange; 
        
        protected override void OnBattleInit()
        {
            var roleInfo = ((RoleObject)BattleEntity).RoleInfo;
            StatsComponentCore.StatSet = new RoleStatSet();
            StatsComponentCore.StatSetProvider = new RoleStatConfigProvider(roleInfo);
            ((RoleStatSet)StatsComponentCore.StatSet).UltimateResource = new EnergyResource(EResourceStatType.Energy, roleInfo.f_maxEnergy, 0);
            base.OnBattleInit();
        }
        
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="amount"></param>
        public void GainResource(float amount)
        {
            UltimateResource.Gain(amount);
            OnResourceChange?.Invoke(UltimateResource.CurrentValue, UltimateResource.MaxValue);
        }

        /// <summary>
        /// 消耗
        /// </summary>
        /// <param name="amount"></param>
        public void ConsumeResource(float amount)
        {
            UltimateResource.Consume(amount);
            OnResourceChange?.Invoke(UltimateResource.CurrentValue, UltimateResource.MaxValue);
        }

        /// <summary>
        /// 消耗所有
        /// </summary>
        public void ConsumeAllResource()
        {
            ConsumeResource(UltimateResource.CurrentValue);
            OnResourceChange?.Invoke(UltimateResource.CurrentValue, UltimateResource.MaxValue);
        }
        
        protected override void OnBattleDestroy()
        {
            
        }
    }
}
