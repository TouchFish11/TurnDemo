using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 战斗界面控制器拓展
/// </summary>
public static class BattleControllerExtension
{
    /// <summary>
    /// 获取伤害文本UI位置
    /// </summary>
    /// <param name="_"></param>
    /// <param name="dmgTarget"></param>
    /// <param name="damageTextXOffsetRange"></param>
    /// <param name="damageTextYOffsetRange"></param>
    /// <returns></returns>
    public static Vector2 GetDamageTextUIPos(this BattleController _, IBattleEntityObject dmgTarget, Vector2 damageTextXOffsetRange, Vector2 damageTextYOffsetRange)
    {
        float x = Random.Range(damageTextXOffsetRange.x, damageTextXOffsetRange.y);
        float y = Random.Range(damageTextYOffsetRange.x, damageTextYOffsetRange.y);
        Vector2 dmgTextOffset = new Vector2(x, y);
        Vector2 pos = default;
        switch (dmgTarget)
        {
            case MonsterObject monster:
                pos = Vector2.up * monster.MonsterInfo.f_dmgTextYOffset + dmgTextOffset;
                break;
            case PlayerObject player:
                pos = Vector2.up * player.RoleInfo.f_dmgTextYOffset + dmgTextOffset;
                break;
        }

        return pos;
    }

    /// <summary>
    /// 获取伤害类型文本
    /// </summary>
    /// <param name="_"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string GetDamgeTypeText(this BattleController _, DamageResult result)
    {
        string dmgTypeText = string.Empty;
        if (result.DamageType == E_DamageType.Direct)
        {
            dmgTypeText = result.IsCrit ? "暴击" : "";
        }
        else
        {
            switch (result.DamageType)
            {
                case E_DamageType.True:
                    dmgTypeText = "真伤";
                    break;
                case E_DamageType.Break:
                    dmgTypeText = "击破";
                    break;
                case E_DamageType.SuperBreak:
                    dmgTypeText = "超击破";
                    break;
                case E_DamageType.Dot:
                    dmgTypeText = "持续伤害";
                    break;
            }
        }

        return dmgTypeText;
    }
}
