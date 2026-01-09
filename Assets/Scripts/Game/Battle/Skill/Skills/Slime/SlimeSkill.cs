using Framework;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSkill : Skill
{
    private static WaitForSeconds _waitForSeconds0_3 = new WaitForSeconds(0.3f);

    private readonly float moveSpeed = 15f;
    private readonly float dis = 2f;

    /// <summary>
    /// 攻击
    /// 目前是怪物使用
    /// </summary>
    public string Attack { get; } = "Attack";
    protected override int DmgCount { get; set; } = 1;

    public SlimeSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler) : base(caster, skillId, postHandler)
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
            --currentDmgCount;
        }
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        Vector3 targetPos = MainTarget.GameObject.transform.position;
        while (Vector3.Distance(Caster.GameObject.transform.position, targetPos) > dis)
        {
            Vector3 nowPos = Caster.GameObject.transform.position;
            Caster.GameObject.transform.position = Vector3.MoveTowards(nowPos, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }

        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为攻击动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().IsName(Attack) && animationComponent.GetCurrentAnimatorStateInfo().normalizedTime >= 0.9f);

        // 优化表现
        yield return _waitForSeconds0_3;

        // 回到起始位置
        targetPos = BattlePoint.Instance.GetMonsterTransByIndex(context.GetMonsterObjectIndex(Caster)).position;
        while (Vector3.Distance(Caster.GameObject.transform.position, targetPos) >= 0.1f)
        {
            Vector3 nowPos = Caster.GameObject.transform.position;
            Caster.GameObject.transform.position = Vector3.MoveTowards(nowPos, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }

        Caster.GameObject.transform.position = targetPos;
    }
}
