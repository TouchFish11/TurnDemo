using Framework;
using System.Collections;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 弱点属性攻击（如物理属性单体技能）
    /// </summary>
    public class WeakPointAttackSkill : Skill
    {
        public WeakPointAttackSkill(int skillId) : base(skillId)
        {

        }

        public override IEnumerator Cast(IBattleContext context)
        {
            LogManager.Log($"{Caster.Name}释放技能：{SkillInfo.f_name}");

            this.Caster.SubActCount();
            //// 计算技能伤害（基于角色攻击力+技能系数）
            //int finalDamage = (int)(Caster.GetProperty(E_DynamicPropertyType.BaseAtk) * DamageCoefficient);

            //for (int i = 0; i < AllTargets.Count; i++)
            //{
            //    // 目标受到伤害（调用角色API）
            //    AllTargets[i].TakeDamage(finalDamage, PropertyType);
            //    LogManager.Log($"{AllTargets[i].Name}受到{finalDamage}点{PropertyType}属性伤害");
            //}

            //// 3. 广播“技能释放事件”（关键：通知其他模块“技能已释放”）
            //Caster.Context.GetEventBus().TriggerEvent(new SkillCastEvent(context, Caster, AllTargets, this, finalDamage, PropertyType));

            yield break;
        }
    }
}
