using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家属性组件
/// </summary>
[ComponentId(nameof(PlayerPropertyComponent))]
public class PlayerPropertyComponent : PropertyComponent
{
    public override void BattleInit(IBattleEntityObject battleEntity)
    {
        base.BattleInit(battleEntity);

        battleProperty = new RoleProperty();
        battleProperty.InitProperty(battleEntity.BattleEntityId);
    }
}
