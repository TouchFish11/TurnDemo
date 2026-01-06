using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HertaNormalSkill : Skill
{
    private float dis = 2f;
    private float dmgTime = 0.24f + 0.04f;

    private string rollState = "Roll";
    private string attackState = "Attack";

    protected override int DmgCount { get; set; } = 1;

    public HertaNormalSkill(int skillId, ISkillCastPostHandler postHandler) : base(skillId, postHandler)
    {

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

        AnimatorStateInfo stateInfo = animationComponent.GetCurrentAnimatorStateInfo();
        while (stateInfo.normalizedTime < stateInfo.length)
        {
            stateInfo = animationComponent.GetCurrentAnimatorStateInfo();
            if (stateInfo.normalizedTime >= dmgTime && currentDmgCount >= 1)
            {
                foreach (IBattleEntityObject battleEntity in AllTargets)
                {
                    DamageCalcManager.CalcSkillDamage(Caster, battleEntity, this.SkillInfo, out DamageResult result);
                    battleEntity.TakeDamage(result);
                    RecoverEnergy();
                    --currentDmgCount;
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.35f);

        // 回到起始位置
        animator.transform.localPosition = Vector3.zero;
    }
}
