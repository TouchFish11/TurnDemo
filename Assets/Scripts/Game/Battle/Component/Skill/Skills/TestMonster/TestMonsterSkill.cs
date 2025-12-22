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
        LogManager.Log($"{Caster.Name}ÊÍ·Å¼¼ÄÜ£º{SkillInfo.f_name}");
        this.Caster.SubActCount();
        yield break;
    }
}
