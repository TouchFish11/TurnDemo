using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家默认释放技能条件
/// </summary>
[CastSkillCondition]
public class PlayerDefaultCastSkillCondition : ICastSkillCondition
{
    public bool CanCast(IBattleEntityObject caster, ISkill skill)
    {
        switch ((E_SkillType)skill.SkillInfo.f_SkillType)
        {
            case E_SkillType.NormalAttack:
                return true;
            case E_SkillType.CombatSkill:
                int tempBP = caster.Context.CurentBattlePointCount;
                // 剩余战机点减去战技花费的战技点满足条件
                if (tempBP - skill.SkillInfo.f_costBP >= 0)
                {
                    return true;
                }
                else
                {
                    // 显示提示框UI
                    ServiceLocator.Instance.Get<IEventCenter>().TriggerEvent(new GlobalMessageEvent("战机点不足，无法释放"));
                    return false;
                }
            case E_SkillType.UltimateSkill:
                // 终结技需判断能量是否足够
                int currentEnergy = caster.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
                int baseEnergy = caster.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.BaseEnergy);
                if (currentEnergy == baseEnergy)
                {
                    return true;
                }
                else
                {
                    // 提示玩家能量不足
                    ServiceLocator.Instance.Get<IEventCenter>().TriggerEvent(new GlobalMessageEvent("能量不足，无法释放终结技"));
                    return false;
                }
            case E_SkillType.EnhancedNormalAttack:
            case E_SkillType.EnhancedCombatSkill:
                return true;
            default:
                return false;
        }
    }
}
