using Framework;
using Game;
using Game.Battle;
using System.Collections;

/// <summary>
/// 基础终结技技能释放后处理器
/// </summary>
public class BaseUltimateSkillCastPostHandler : ISkillCastPostHandler
{
    public IEnumerator OnHnadle(ISkill skill)
    {
        // FIXME：需要修改，因为新增了韧性条恢复逻辑，所以导致释放终结技后的玩家不一定是当前玩家，有可能是恢复韧性条期间的怪物实体，所以这里需要修改
        IBattleContext context = skill.Caster.Context;
        IBattleEntityObject currentEntity = context.GetCurrentEntity();

        if (currentEntity is MonsterObject)
        {
            // 更新为怪物行动提示

            // 切换

            yield break;
        }
        else
        {
            // 判断当前玩家是否还有行动次数
            if (currentEntity.CanAct)
            {
                SkillInfo currentEntitySkillInfo = currentEntity.GetComponent<SkillComponent>().GetNormalAttackSkill().SkillInfo;
                // 用于玩家终结技结束后恢复UI
                context.GetEventBus().TriggerEvent(new UltimateReleaseOverEvent(context, currentEntity));
                BattleUIScheduler.Instance.UpdateCameraAndMarkerAndMonsterUI(context, currentEntity, currentEntitySkillInfo);
            }
        }
    }
}
