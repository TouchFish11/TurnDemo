using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// FireFly终结技
/// </summary>
public class FireFlyUltimateSkill : UltimateSkill
{
    private static WaitForSeconds _waitForSeconds0_25 = new WaitForSeconds(0.25f);
    private readonly string ultimateAttackState = "UltimateAttack";

    protected override int DmgCount { get; set; } = 3;

    private bool isAddStatus;

    public FireFlyUltimateSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
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

        if (!isAddStatus)
        {
            StatusAddStrategy?.ToAdd(Caster, AllTargets, statusIds);
            isAddStatus = true;
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
        // 传送到主目标身前
        Vector3 targetPos = MainTarget.GameObject.transform.position;
        Caster.GameObject.transform.position = targetPos - Vector3.forward;

        yield return _waitForSeconds0_25;

        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为战技动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().IsName(ultimateAttackState));
        // 等待动画结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().normalizedTime >= 0.9f);

        // 回到起始位置
        targetPos = BattlePoint.Instance.GetPlayerTransByIndex(Caster.EntityPosIndex).position;
        Caster.GameObject.transform.position = targetPos;

        yield return _waitForSeconds0_25;
    }

    protected override IEnumerator OnPostCast()
    {
        isAddStatus = false;
        return base.OnPostCast();
    }
}
