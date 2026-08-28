using System;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem.Providers
{
    /// <summary>
    /// 怪物属性配置提供器接口
    /// </summary>
    public class MonsterStatConfigProvider : IStatConfigProvider
    {
        private readonly MonsterInfo _monsterInfo;

        public MonsterStatConfigProvider(MonsterInfo monsterInfo)
        {
            _monsterInfo = monsterInfo;
        }
        
        public float GetConfigValue(EStatType statType)
        {
            return statType switch
            {
                EStatType.Hp => _monsterInfo.f_baseHp,
                EStatType.Def => _monsterInfo.f_baseDef,
                EStatType.Atk => _monsterInfo.f_baseAtk,
                EStatType.Speed => _monsterInfo.f_baseSpeed,
                EStatType.Crit => 50,
                EStatType.CritDmg => 100,
                _ => throw new ArgumentOutOfRangeException(nameof(statType), statType, null)
            };
        }
    }
}
