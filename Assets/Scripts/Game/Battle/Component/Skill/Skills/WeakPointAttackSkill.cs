using Framework;
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 弱点属性攻击（如物理属性单体技能）
    /// </summary>
    public class WeakPointAttackSkill : ISkill
    {
        public string Name { get; }

        public float DamageCoefficient { get; }

        public E_PropertyType PropertyType { get; }

        public WeakPointAttackSkill(string name, float damageCoeff, E_PropertyType propertyType)
        {
            Name = name;
            DamageCoefficient = damageCoeff;
            PropertyType = propertyType;
        }

        public void Cast(IBattleContext context, IBattleEntityObject caster, List<IBattleEntityObject> targets)
        {
            LogMgr.Log($"\n{caster.Name}释放技能：{Name}");
            // 计算技能伤害（基于角色攻击力+技能系数）
            int finalDamage = (int)(caster.GetField(E_FieldType.Attack) * DamageCoefficient);

            for (int i = 0; i < targets.Count; i++)
            {
                // 目标受到伤害（调用角色API）
                targets[i].TakeDamage(finalDamage, PropertyType);
                LogMgr.Log($"{targets[i].Name}受到{finalDamage}点{PropertyType}属性伤害");
            }

            // 3. 广播“技能释放事件”（关键：通知其他模块“技能已释放”）
            BattleEventCenter.TriggerEvent(new SkillCastEvent(context, caster, targets, this, finalDamage, PropertyType));
        }
    }
}
