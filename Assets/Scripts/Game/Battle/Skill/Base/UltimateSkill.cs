using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// 终结技技能
/// </summary>
public abstract class UltimateSkill : PlayerSkill
{
    private ISkillComponent skillComponent;

    public UltimateSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {
        skillComponent = Caster.GetComponent<SkillComponent>();
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        // 终结技释放前
        OnPreUltimateCast(context);
        // 等待输入
        yield return new WaitUntil(() => skillComponent.IsRelease);
        // 确定技能作用目标
        ServiceLocator.Get<ISkillManager>().InitSkillTarget(this);
        // 禁用目标选择
        ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
        // 隐藏相关UI内容
        BattleUIScheduler.Instance.UltimateCasting();
        // 终结技释放
        yield return OnUltimateCast(context);
    }

    /// <summary>
    /// 终结技释放前
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnPreUltimateCast(IBattleContext context)
    {
        // 激活玩家相机
        BattlePoint.Instance.ActiveCamera(Caster);
        // 更新看向
        context.GetTurnManager().UpdateEntityLookAt(Caster);
        // 激活目标选择
        ServiceLocator.Get<ITargetSelectManager>().ActiveSelectTarget();
        // 主动更新目标选择
        ServiceLocator.Get<ITargetSelectManager>().SelectTarget(context, Caster, SkillInfo, IFactory.GetTypeInstance<TargetSelectStrategyFactory, PlayerBaseTargetSelectStrategy>());
        // 更新终结技相关UI显示
        BattleUIScheduler.Instance.UltimateTriggerChangeUI(Caster, SkillInfo);
        // 暂时清空能量，更新能量显示
        PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
    }

    /// <summary>
    /// 终结技释放
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract IEnumerator OnUltimateCast(IBattleContext context);
}
