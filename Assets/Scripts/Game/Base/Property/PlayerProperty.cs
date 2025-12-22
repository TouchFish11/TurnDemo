using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ÕÊº“ Ù–‘
/// </summary>
public class PlayerProperty : BattleProperty
{
    public override void InitProperty(int id)
    {
        RoleInfo roleInfo = BinaryDataMgr.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];

        baseHp = roleInfo.f_baseHp;
        baseAtk = roleInfo.f_baseAtk;
        baseDef = roleInfo.f_baseDef;
        baseSpeed = roleInfo.f_baseSpeed;
    }
}
