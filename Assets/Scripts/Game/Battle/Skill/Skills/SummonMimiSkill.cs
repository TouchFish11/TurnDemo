using Framework;
using System.Collections;

namespace Game.Battle
{
    public class SummonMimiSkill : Skill
    {
        protected override int DmgCount { get; set; } = 1;

        public SummonMimiSkill(int skillId, ISkillCastPostHandler postHandler) : base(skillId, postHandler)
        {

        }

        protected override IEnumerator OnCast(IBattleContext context)
        {
            LogManager.Log($"{Caster.GameObject.name}释放技能：{SkillInfo.f_name}");

            // 播放动画
            context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));

            yield break;

            // 调用召唤物组件API，创建神君（初始行动次数=2，配置表读取）
            //caster.GetBattleComponent<SummonComponent>().CreateSummon<MimiSummon>();

            // 广播“技能释放事件”（触发召唤物协同攻击）(可选)
            //BattleEventBus.TriggerEvent(new SkillCastEvent(_context, caster, target, this, DamageCoefficient * caster.GetAttribute(AttributeType.BaseAtk), AttackAttribute));
        }
    }
}
