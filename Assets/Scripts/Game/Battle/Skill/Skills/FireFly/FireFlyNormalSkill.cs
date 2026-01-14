using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// FireFly普攻
/// </summary>
public class FireFlyNormalSkill : PlayerSkill
{
    private static WaitForSeconds _waitForSeconds0_35 = new WaitForSeconds(0.35f);

    private readonly string rollState = "Roll";
    private readonly string attackState = "Attack";

    protected override int DmgCount { get; set; } = 1;

    public FireFlyNormalSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {
        Caster.GetComponentInChildren<AnimationTrigger>().OnAttack += OnAttack;
    }

    private void OnAttack(int skillId)
    {
        if (skillId != SkillInfo.f_id)
        {
            return;
        }

        foreach (IBattleEntityObject target in AllTargets)
        {
            DamageCalcManager.CalcSkillDamage(Caster, target, this.SkillInfo, out DamageResult result);
            // 处理伤害
            target.TakeDamage(result);
            // 恢复能量
            RecoverEnergy();
            --currentDmgCount;
        }
        StatusAddStrategy?.ToAdd(Caster, AllTargets, statusIds);
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        LogManager.Log($"{Caster.GameObject.name}释放技能：{SkillInfo.f_name}");

        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));

        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        Animator animator = animationComponent.GetAnimator();

        // 等待动画切换为翻滚动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().IsName(rollState));

        // 开始匹配目标
        Vector3 matchPos = MainTarget.GameObject.transform.position - Vector3.forward;
        Quaternion matchRot = Quaternion.identity;
        MatchTargetWeightMask mask = new MatchTargetWeightMask(new Vector3(1, 0, 1), 0);
        animator.MatchTarget(matchPos, matchRot, AvatarTarget.Body, mask, 0.28f);

        // 等待动画切换为普攻动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().IsName(attackState));
        // 等待动画结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().normalizedTime >= 0.9f);

        yield return _waitForSeconds0_35;

        // 回到起始位置
        animator.transform.localPosition = Vector3.zero;
    }
}
