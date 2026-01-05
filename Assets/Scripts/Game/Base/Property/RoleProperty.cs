using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色属性
/// </summary>
public class RoleProperty : BattleProperty
{
    // 基础属性
    protected int baseEnergy;   // 基础能量



    // 动态属性
    protected int currentEnergy;    // 当前能量


    public override void InitProperty(int id)
    {
        base.InitProperty(id);
        RoleInfo roleInfo = BinaryDataManager.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];

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



    public int CurrentEnergy { get => currentEnergy; set => currentEnergy = Mathf.Clamp(value, default, baseEnergy); }

}
