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
        base.InitProperty(id);
        MonsterInfo monsterInfo = BinaryDataMgr.Instance.GetConfig<MonsterInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];

        baseHp = monsterInfo.f_baseHp;
        baseAtk = monsterInfo.f_baseAtk;
        baseDef = monsterInfo.f_baseDef;
        baseSpeed = monsterInfo.f_baseSpeed;

        currentHp = maxHp = baseHp;
        currentSpeed = baseSpeed;
        totalAtk = baseAtk;
        totalDef = baseDef;
        totalCrit = baseCrit;
        totalCritDmg = baseCritDmg;
    }
}
