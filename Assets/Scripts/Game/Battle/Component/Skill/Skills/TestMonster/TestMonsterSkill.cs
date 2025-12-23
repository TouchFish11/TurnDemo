using Framework;
using Game.Battle;
using System.Collections;

public class TestMonsterSkill : Skill
{
    public TestMonsterSkill(int skillId) : base(skillId)
    {

    }

    public override IEnumerator Cast(IBattleContext context)
    {
        yield return base.Cast(context);

        foreach (var item in AllTargets)
        {
            DamageCalcManager.Instance.CalcDamage(Caster, item, this, out DamageResult result);
            item.TakeDamage(result);
        }

        LogManager.Log($"{Caster.GameObject.name}ÊÍ·Å¼¼ÄÜ£º{SkillInfo.f_name}");
    }
}
