using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

/// <summary>
/// 玩家角色技能组件
/// </summary>
[ComponentId(nameof(PlayerSkillComponent))]
public class PlayerSkillComponent : SkillComponent
{
    public override bool IsRelease { get; protected set; }

    public override void BattleInit(IBattleEntityObject battleEntity)
    {
        base.BattleInit(battleEntity);

        AddCastCondition(SkillManager.GetCastSkillCondition<PlayerDefaultCastSkillCondition>());
    }

    /// <summary>
    /// 释放终结技
    /// 暂时使用
    /// </summary>
    public void ReleaseUltimate()
    {
        IsRelease = true;
    }
}
