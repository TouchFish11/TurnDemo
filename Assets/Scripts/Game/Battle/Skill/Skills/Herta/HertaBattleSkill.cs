using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HertaBattleSkill : Skill
{
    public HertaBattleSkill(int skillId, ISkillCastPostHandler postHandler) : base(skillId, postHandler)
    {

    }
    protected override IEnumerator OnCast(IBattleContext context)
    {
        LogManager.Log($"{Caster.GameObject.name}释放技能：{SkillInfo.f_name}");

        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));

        foreach (IBattleEntityObject battleEntity in AllTargets)
        {
            MulTest(battleEntity, 2);
            RecoverEnergy();
        }

        yield break;
    }
}
