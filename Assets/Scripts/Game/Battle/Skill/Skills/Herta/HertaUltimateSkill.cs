using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// Herta终结技
/// </summary>
public class HertaUltimateSkill : UltimateSkill
{
    private readonly string ultimateAttackState = "UltimateAttack";

    protected override int DmgCount { get; set; } = 3;

    public HertaUltimateSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {
        Caster.GetComponentInChildren<AnimationTrigger>().OnAttack += OnAttack;
    }

    private void OnAttack(int skillId)
    {
        if (skillId != SkillInfo.f_id)
        {
            return;
        }

        int index = 0;
        foreach (IBattleEntityObject battleEntity in AllTargets)
        {
            DamageCalcManager.CalcSkillDamage(Caster, battleEntity, this.SkillInfo, out DamageResult result);
            battleEntity.TakeDamage(result);
            RecoverEnergy();
            ++index;
        }
    }

    protected override void OnPreUltimateCast(IBattleContext context)
    {
        base.OnPreUltimateCast(context);

        // 播放预备动画：玩家终结技pose、终结技动画
        Caster.GetComponent<BattleAnimationComponent>().SetUltimatePose(); 
    }

    protected override IEnumerator OnUltimateCast(IBattleContext context)
    {
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为战技动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().IsName(ultimateAttackState));
        // 等待动画结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().normalizedTime >= 0.9f);
    }
}
