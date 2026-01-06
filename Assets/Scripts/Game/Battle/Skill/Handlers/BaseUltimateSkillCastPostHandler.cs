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
        IBattleContext context = skill.Caster.Context;
        IBattleEntityObject currentEntity = context.GetCurrentEntity();
        // 判断当前玩家是否还有行动次数
        if (currentEntity.CanAct)
        {
            SkillInfo currentEntitySkillInfo = currentEntity.GetComponent<SkillComponent>().GetNormalAttackSkill().SkillInfo;
            // 用于玩家终结技结束后恢复UI
            context.GetEventBus().TriggerEvent(new UltimateReleaseOverEvent(context, currentEntity));
            BattleUIScheduler.Instance.UpdateCameraAndMarkerAndMonsterUI(context, currentEntity, currentEntitySkillInfo);
        }
        yield break;
    }
}
