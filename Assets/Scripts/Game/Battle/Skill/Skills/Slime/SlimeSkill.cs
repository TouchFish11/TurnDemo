using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSkill : Skill
{
    private float moveSpeed = 15f;
    private float dis = 2f;
    private float dmgTime = 0.08f;

    /// <summary>
    /// 攻击
    /// 目前是怪物使用
    /// </summary>
    public int Attack { get; } = Animator.StringToHash("Attack");
    protected override int DmgCount { get; set; } = 1;

    public SlimeSkill(int skillId, ISkillCastPostHandler postHandler) : base(skillId, postHandler)
    {

    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        LogManager.Log($"{Caster.GameObject.name}释放技能：{SkillInfo.f_name}");

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
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo().shortNameHash == Attack);

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
                    --currentDmgCount;
                }
            }

            yield return null;
        }

        // 优化表现
        yield return new WaitForSeconds(0.3f);

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
