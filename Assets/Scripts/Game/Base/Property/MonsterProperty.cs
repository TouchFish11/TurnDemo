using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// π÷ŒÔ Ù–‘
/// </summary>
public class MonsterProperty : BattleProperty
{
    public override void InitProperty(int id)
    {
        MonsterInfo roleInfo = BinaryDataMgr.Instance.GetConfig<MonsterInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];

        baseHp = roleInfo.f_baseHp;
        baseAtk = roleInfo.f_baseAtk;
        baseDef = roleInfo.f_baseDef;
        baseSpeed = roleInfo.f_baseSpeed;
    }
}
