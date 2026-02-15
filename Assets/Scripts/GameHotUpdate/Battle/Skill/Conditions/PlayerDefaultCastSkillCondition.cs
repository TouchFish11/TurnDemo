using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Service;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Condition;
using Game.Battle.Skill.Enum;
using GameHotUpdate.Battle.Property;

namespace GameHotUpdate.Battle.Skill.Conditions
{
    /// <summary>
    /// ���Ĭ���ͷż�������
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
                    int tempBP = caster.Context.CurentBattlePointCount;
                    // ʣ��ս�����ȥս�����ѵ�ս������������
                    if (tempBP - skill.SkillInfo.f_costBP >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        // ��ʾ��ʾ��UI
                        ServiceLocator.Get<IEventCenter>().TriggerEvent(new GlobalMessageEvent() { Message = "ս���㲻�㣬�޷��ͷ�" });
                        return false;
                    }
                case E_SkillType.UltimateSkill:
                    // �սἼ���ж������Ƿ��㹻
                    int currentEnergy = caster.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
                    int baseEnergy = caster.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.BaseEnergy);
                    if (currentEnergy == baseEnergy)
                    {
                        return true;
                    }
                    else
                    {
                        // ��ʾ�����������
                        ServiceLocator.Get<IEventCenter>().TriggerEvent(new GlobalMessageEvent() { Message = "�������㣬�޷��ͷ��սἼ" });
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
}
