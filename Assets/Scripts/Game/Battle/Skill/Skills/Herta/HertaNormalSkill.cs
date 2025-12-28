using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HertaNormalSkill : Skill
{
    public HertaNormalSkill(int skillId) : base(skillId)
    {

    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        LogManager.Log($"{Caster.GameObject.name}ÊÍ·Å¼¼ÄÜ£º{SkillInfo.f_name}");

        foreach (IBattleEntityObject battleEntity in AllTargets)
        {
            MulTest(battleEntity, 1);
        }

        yield break;
    }
}
