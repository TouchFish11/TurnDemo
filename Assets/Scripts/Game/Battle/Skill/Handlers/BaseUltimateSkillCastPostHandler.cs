using Game.Battle;
using System.Collections;


/// <summary>
/// 基础终结技技能释放后处理器
/// </summary>
[SkillCastPostHandler]
public class BaseUltimateSkillCastPostHandler : ISkillCastPostHandler
{
    public IEnumerator OnHnadle(ISkill skill)
    {
        // 判断当前玩家是否还有行动次数
        if (skill.Caster.CanAct)
        {
            // 用于玩家终结技结束后恢复UI
            skill.Caster.Context.GetEventBus().TriggerEvent(new UltimateReleaseOverEvent(skill.Caster.Context));
        }
        yield break;
    }
}
