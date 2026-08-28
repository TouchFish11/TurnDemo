using System;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem.Providers
{
    /// <summary>
    /// 角色属性配置提供器接口
    /// </summary>
    public class RoleStatConfigProvider : IStatConfigProvider
    {
        private readonly RoleInfo _roleInfo;
        
        public RoleStatConfigProvider(RoleInfo roleInfo)
        {
            _roleInfo = roleInfo;
        }
        
        public float GetConfigValue(EStatType statType)
        {
            return statType switch
            {
                EStatType.Hp => _roleInfo.f_baseHp,
                EStatType.Def => _roleInfo.f_baseDef,
                EStatType.Atk => _roleInfo.f_baseAtk,
                EStatType.Speed => _roleInfo.f_baseSpeed,
                EStatType.Crit => 50,
                EStatType.CritDmg => 100,
                _ => throw new ArgumentOutOfRangeException(nameof(statType), statType, null)
            };
        }
    }
}
