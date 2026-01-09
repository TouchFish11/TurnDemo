using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyBattleSkill : Skill
{
    private readonly string battleAttackState = "BattleAttack";

    protected override int DmgCount { get; set; } = 1;

    public FireFlyBattleSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler) : base(caster, skillId, postHandler)
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
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        LogManager.Log($"{Caster.GameObject.name}释放技能：{SkillInfo.f_name}");
        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为战技动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().IsName(battleAttackState));
        // 等待动画结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().normalizedTime >= 0.9f);
    }
}
