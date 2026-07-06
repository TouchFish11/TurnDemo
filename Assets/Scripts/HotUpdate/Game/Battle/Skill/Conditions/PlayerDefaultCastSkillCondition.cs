using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;

using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Property;

namespace HotUpdate.Game.Battle.Skill.Conditions
{
    /// <summary>
    /// 角色默认释放技能条件
    /// </summary>
    public class PlayerDefaultCastSkillCondition : ICastSkillCondition
    {
        [Inject] private IEventCenter _eventCenter;
        
        public bool CanCast(IBattleEntityObject caster, SkillInfo skillInfo)
        {
            switch ((E_SkillType)skillInfo.f_SkillType)
            {
                case E_SkillType.NormalAttack:
                    return true;
                case E_SkillType.CombatSkill:
                    var tempBP = caster.Context.CurentBattlePointCount;
                    // 战技点数大于消耗点数
                    if (tempBP - skillInfo.f_costBP >= 0)
                    {
                        return true;
                    }

                    // 全局提示
                    _eventCenter.TriggerEvent(new GlobalMessageEvent{Message = "战技点不足无法释放"});
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
                    _eventCenter.TriggerEvent(new GlobalMessageEvent{Message = "能量不足无法释放"});
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
