using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家属性组件
/// </summary>
public class PlayerPropertyComponent : PropertyComponent
{
    public override void BattleInit(IBattleEntityObject battleEntity)
    {
        base.BattleInit(battleEntity);

        battleProperty = new PlayerProperty();
        battleProperty.InitProperty(battleEntity.BattleEntityId);
    }
}
