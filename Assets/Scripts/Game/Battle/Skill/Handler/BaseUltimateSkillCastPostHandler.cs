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

        if (currentEntity is not PlayerObject)
        {
            yield break;
        }

        // 判断当前玩家是否还有行动次数
        if (currentEntity.CanAct)
        {
            SkillInfo currentEntitySkillInfo = currentEntity.GetComponent<SkillComponent>().GetSkills().Find((skill) => skill.SkillInfo.f_SkillType == (int)E_SkillType.NormalAttack).SkillInfo;
            // 玩家终结技结束后恢复UI
            ServiceLocator.Get<IUIManager>().GetView<BattleController>().GetBattleUI().SetActTipActive(BattleUIManager.E_ActTipType.Hide);
            ServiceLocator.Get<IUIManager>().GetView<BattleController>().GetBattleUI().UpdateOperator(currentEntity, IFactory.GetTypeInstance<SkillKeyUIDataProviderFactory, BaseSkillKeyUIDataProvider>());
            // 激活玩家相机，传入的必须是玩家对象
            BattlePoint.Instance.ActiveCamera(currentEntity);
            // 更新看向
            context.GetTurnManager().UpdateEntityLookAt(currentEntity);
            // 更新目标选择
            ServiceLocator.Get<ITargetSelectManager>().SelectTarget(context, currentEntity, currentEntitySkillInfo, IFactory.GetTypeInstance<TargetSelectStrategyFactory, PlayerBaseTargetSelectStrategy>());
            // 更新怪物血条位置
            ServiceLocator.Get<IUIManager>().GetView<BattleController>().GetUIInitializer().InitMonsterUI(context.GetMonsterObjects());
        }
    }
}
