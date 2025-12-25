using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

public class TestMonsterSkill : Skill
{
    public TestMonsterSkill(int skillId) : base(skillId)
    {

    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        LogManager.Log($"{Caster.GameObject.name}ÊÍ·Å¼¼ÄÜ£º{SkillInfo.f_name}");

        foreach (IBattleEntityObject battleEntity in AllTargets)
        {
            MulTest(battleEntity, 1);
        }

        yield return new WaitForSeconds(0.5f);
    }
}
