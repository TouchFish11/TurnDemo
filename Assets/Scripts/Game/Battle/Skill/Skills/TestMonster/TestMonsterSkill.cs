using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

public class TestMonsterSkill : Skill
{
    public TestMonsterSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {

    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        LogManager.Log($"{Caster.GameObject.name}释放技能：{SkillInfo.f_name}");

        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));


        yield break;
    }
}
