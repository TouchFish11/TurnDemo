using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// FireFly战技
/// </summary>
public class FireFlyBattleSkill : PlayerSkill
{
    private readonly string battleAttackState = "BattleAttack";

    protected override int DmgCount { get; set; } = 1;

    public FireFlyBattleSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {
        Caster.GetComponentInChildren<AnimationTrigger>().OnAttack += OnAttack;
    }

    private void OnAttack(int skillId)
    {
        if (skillId != SkillInfo.f_id)
        {
            return;
        }

        foreach (IBattleEntityObject battleEntity in AllTargets)
        {
            DamageCalcManager.CalcSkillDamage(Caster, battleEntity, this.SkillInfo, out DamageResult result);
            battleEntity.TakeDamage(result);
            RecoverEnergy();
            --currentDmgCount;
        }

        foreach (int id in statusIds)
        {
            IStatus status = ServiceLocator.Get<IFactoryManager>().GetFactory<StatusFactory>().GetStatus(id);
            status.InitStatus(Caster, Caster, id);
            Caster.GetComponent<StatusComponent>().AddStatus(status);
        }
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为战技动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().IsName(battleAttackState));
        // 等待动画结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().normalizedTime >= 0.9f);
    }
}
