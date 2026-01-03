using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家角色技能组件
/// </summary>
[ComponentId(nameof(PlayerSkillComponent))]
public class PlayerSkillComponent : SkillComponent
{
    private bool isRelease;

    public override void BattleInit(IBattleEntityObject battleEntity)
    {
        base.BattleInit(battleEntity);

        AddCastCondition(SkillManager.GetCastSkillCondition<PlayerDefaultCastSkillCondition>());
    }

    /// <summary>
    /// 释放终结技
    /// </summary>
    /// <param name="skillId"></param>
    public void CastUltimateSkill(int skillId)
    {
        if (skills.TryGetValue(skillId, out var skill))
        {
            if (CanCast(skill))
            {
                isRelease = false;
                // 更新界面UI显示
                this.BattleEntity.Context.GetEventBus().TriggerEvent(new ShowUltimateUIEvent(this.BattleEntity.Context, skill, this.BattleEntity));
                // 等待输入
                StartCoroutine(WaitForRelease(skill));
            }
        }
    }

    /// <summary>
    /// 等待释放
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForRelease(ISkill skill)
    {
        yield return new WaitUntil(() => isRelease);
        // 释放终结技
        CastSkill(skill);
    }

    public void ReleaseUltimate()
    {
        isRelease = true;
    }
}
