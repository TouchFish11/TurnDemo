using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// 终结技技能
/// </summary>
public abstract class UltimateSkill : Skill
{
    private ISkillComponent skillComponent;

    public UltimateSkill(int skillId) : base(skillId)
    {

    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        skillComponent = Caster.GetComponent<SkillComponent>();
        // 更新界面UI显示
        context.GetEventBus().TriggerEvent(new ShowUltimateUIEvent(context, this, Caster));
        // 等待输入
        yield return new WaitUntil(() => skillComponent.IsRelease);
        // 终结技释放前
        OnPreUltimateCast(context);
        // 终结技释放
        yield return OnUltimateCast(context);
        // TODO：暂时直接触发对应动画，之后根据具体技能的时机触发
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        // 终结技释放后
        OnPostUltimateCast(context);
    }

    /// <summary>
    /// 终结技释放前
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnPreUltimateCast(IBattleContext context)
    {
        // 更新能量显示
        this.Caster.GetComponent<PlayerPropertyComponent>().SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
    }

    /// <summary>
    /// 终结技释放
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract IEnumerator OnUltimateCast(IBattleContext context);

    /// <summary>
    /// 终结技释放后
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnPostUltimateCast(IBattleContext context)
    {
        // TODO：用于玩家终结技结束后恢复UI
        context.GetEventBus().TriggerEvent(new UltimateReleaseOverEvent(context));
    }

}
