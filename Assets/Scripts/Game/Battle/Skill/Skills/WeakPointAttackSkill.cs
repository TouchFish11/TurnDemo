using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 弱点属性攻击（测试技能）
    /// </summary>
    public class WeakPointAttackSkill : Skill
    {
        public WeakPointAttackSkill(int skillId) : base(skillId)
        {

        }

        protected override IEnumerator OnCast(IBattleContext context)
        {
            LogManager.Log($"{Caster.GameObject.name}释放技能：{SkillInfo.f_name}");

            foreach (IBattleEntityObject battleEntity in AllTargets)
            {
                MulTest(battleEntity, 2);
            }
            // 广播“技能释放事件”（关键：通知其他模块“技能已释放”）
            // Caster.Context.GetEventBus().TriggerEvent(new SkillCastEvent(context, Caster, AllTargets, this, finalDamage, ElementType));

            yield break;
        }
    }
}
