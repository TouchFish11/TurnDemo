using Game.Battle;
using System.Collections;

/// <summary>
/// 基础技能释放后处理器
/// </summary>
public class BaseSkillCastPostHandler : ISkillCastPostHandler
{
    public IEnumerator OnHnadle(ISkill skill)
    {
        // 减少行动次数
        skill.Caster.SubActCount();
        yield break;
    }
}
