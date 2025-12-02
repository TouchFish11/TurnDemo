using Framework;
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;

namespace Game.Battle
{
    public class SummonMimiSkill : ISkill
    {
        // 配表
        public string Name => "召唤迷迷技能";

        public float DamageCoefficient => throw new System.NotImplementedException();

        public E_PropertyType PropertyType => throw new System.NotImplementedException();

        public void Cast(IBattleContext context, IBattleEntityObject caster, List<IBattleEntityObject> targets)
        {
            LogMgr.Log($"\n{caster.Name}释放技能：{Name}");

            // 调用召唤物组件API，创建神君（初始行动次数=2，配置表读取）
            //caster.GetBattleComponent<SummonComponent>().CreateSummon<MimiSummon>();

            // 广播“技能释放事件”（触发召唤物协同攻击）(可选)
            //BattleEventCenter.TriggerEvent(new SkillCastEvent(context, caster, target, this, DamageCoefficient * caster.GetAttribute(AttributeType.Attack), AttackAttribute));
        }
    }
}
