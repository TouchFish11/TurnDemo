using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物属性组件
/// </summary>
public class MonsterPropertyComponent : PropertyComponent
{
    public override void BattleInit(IBattleEntityObject battleEntity)
    {
        base.BattleInit(battleEntity);

        battleProperty = new MonsterProperty();
        battleProperty.InitProperty(battleEntity.BattleEntityId);
    }
}
