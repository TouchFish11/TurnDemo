using UnityEngine;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 角色属性
    /// </summary>
    public class RoleProperty : BattleProperty
    {
        // 基础能量
        protected int baseEnergy;
        
        // 当前能量
        protected int currentEnergy;
        
        public void InitProperty(RoleInfo roleInfo)
        {
            battleId = roleInfo.f_id;
            
            baseHp = roleInfo.f_baseHp;
            baseAtk = roleInfo.f_baseAtk;
            baseDef = roleInfo.f_baseDef;
            baseSpeed = roleInfo.f_baseSpeed;
            baseEnergy = roleInfo.f_maxEnergy;

            currentHp = maxHp = baseHp;
            currentSpeed = baseSpeed;
            totalAtk = baseAtk;
            totalDef = baseDef;
            totalCrit = baseCrit;
            totalCritDmg = baseCritDmg;

            currentEnergy = 0;
        }


        public int BaseEnergy => baseEnergy;



        public int CurrentEnergy { get => currentEnergy; set => currentEnergy = Mathf.Clamp(value, 0, baseEnergy); }

    }
}
