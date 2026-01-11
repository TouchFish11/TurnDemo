using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 终结技技能
/// </summary>
public abstract class UltimateSkill : Skill
{
    private ISkillComponent skillComponent;

    public UltimateSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {

    }

    public override void Init(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
    {
        base.Init(mainTarget, allTargets);
        skillComponent = Caster.GetComponent<SkillComponent>();
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        // 终结技释放前
        OnPreUltimateCast(context);
        // 等待输入
        yield return new WaitUntil(() => skillComponent.IsRelease);
        // 隐藏UI
        ServiceLocator.Get<IUIManager>().GetView<BattleController>().GetBattleUI().HideOperator(false);
        // 终结技释放
        yield return OnUltimateCast(context);
    }

    /// <summary>
    /// 终结技释放前
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnPreUltimateCast(IBattleContext context)
    {
        // 更新界面UI显示
        BattleUIScheduler.Instance.ShowUltimatePaiting(Caster, SkillInfo);
        BattleUIScheduler.Instance.UpdateCameraAndMarkerAndMonsterUI(context, Caster, SkillInfo);
        // TODO：暂时清空能量，更新能量显示
        PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
    }

    /// <summary>
    /// 终结技释放
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract IEnumerator OnUltimateCast(IBattleContext context);
}
