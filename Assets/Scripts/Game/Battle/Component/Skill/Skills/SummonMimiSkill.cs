using Framework;
using GameLogic.BattleMoudule.Entity;
using System.Collections;
using System.Collections.Generic;

namespace Game.Battle
{
    public class SummonMimiSkill : Skill
    {
        public override IEnumerator Cast(IBattleContext context)
        {
            LogManager.Log($"{Caster.Name}释放技能：{SkillInfo.f_name}");

            Caster.DisableAct();

            yield break;

            // 调用召唤物组件API，创建神君（初始行动次数=2，配置表读取）
            //caster.GetBattleComponent<SummonComponent>().CreateSummon<MimiSummon>();

            // 广播“技能释放事件”（触发召唤物协同攻击）(可选)
            //BattleEventBus.TriggerEvent(new SkillCastEvent(context, caster, target, this, DamageCoefficient * caster.GetAttribute(AttributeType.Attack), AttackAttribute));
        }
    }
}
