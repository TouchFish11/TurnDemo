using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Service;
using HotUpdate.Battle.Property;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Property;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Skill.Conditions
{
    /// <summary>
    /// 角色默认释放技能条件
    /// </summary>
    public class PlayerDefaultCastSkillCondition : ICastSkillCondition
    {
        public bool CanCast(IBattleEntityObject caster, ISkill skill)
        {
            switch ((E_SkillType)skill.SkillInfo.f_SkillType)
            {
                case E_SkillType.NormalAttack:
                    return true;
                case E_SkillType.CombatSkill:
                    var tempBP = caster.Context.CurentBattlePointCount;
                    // 战技点数大于消耗点数
                    if (tempBP - skill.SkillInfo.f_costBP >= 0)
                    {
                        return true;
                    }

                    // 全局提示
                    ServiceLocator.Get<IEventCenter>().TriggerEvent(new GlobalMessageEvent{Message = "战技点不足无法释放"});
                    return false;
                case E_SkillType.UltimateSkill:
                    // 判断能量释放足够
                    var currentEnergy = caster.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
                    var baseEnergy = caster.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.BaseEnergy);
                    if (currentEnergy == baseEnergy)
                    {
                        return true;
                    }

                    // 全局提示
                    ServiceLocator.Get<IEventCenter>().TriggerEvent(new GlobalMessageEvent{Message = "能量不足无法释放"});
                    return false;
                case E_SkillType.EnhancedNormalAttack:
                case E_SkillType.EnhancedCombatSkill:
                    return true;
                case E_SkillType.Monster:
                default:
                    return false;
            }
        }
    }
}
