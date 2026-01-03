using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物技能组件
/// </summary>
[ComponentId(nameof(MonsterSkillComponent))]
public class MonsterSkillComponent : SkillComponent
{
    public override void BattleInit(IBattleEntityObject battleEntity)
    {
        base.BattleInit(battleEntity);

        AddCastCondition(SkillManager.GetCastSkillCondition<MonsterDefaultCastSkillCondition>());
    }
}
