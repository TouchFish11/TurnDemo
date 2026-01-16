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

    private Vector3 localVfx = new Vector3(-90, 180, 0);
    private Transform vfxTrans;

    public FireFlyNormalSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {
        //vfxTrans = (Caster as FireFly).VFXTrans;

        Caster.GetComponentInChildren<AnimationTrigger>().OnAttack += OnAttack;
    }

    private void OnAttack(int skillId)
    {
        //if (skillId != SkillInfo.f_id)
        //{
        //    return;
        //}

        //foreach (IBattleEntityObject target in AllTargets)
        //{
        //    // 碰撞特效
        //    projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
        //    vFXInfo = new VFXInfo();
        //    ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_BlueHit, projectileTrans, default, vFXInfo);
        //}

        //StatusAddStrategy?.ToAdd(Caster, AllTargets, statusIds);
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));

        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        Animator animator = animationComponent.GetAnimator();

        // 等待动画切换为翻滚动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(rollState));

        // 生成特效
        projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
        vFXInfo = new VFXInfo();
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_NormalSkill_Wave, projectileTrans, default, vFXInfo);

        // 开始匹配目标
        Vector3 matchPos = MainTarget.GameObject.transform.position - Vector3.forward * 1.5f;
        Quaternion matchRot = Quaternion.identity;
        MatchTargetWeightMask mask = new MatchTargetWeightMask(new Vector3(1, 0, 1), 0);
        animator.MatchTarget(matchPos, matchRot, AvatarTarget.Body, mask, 0.28f);

        // 等待动画切换为普攻动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(attackState));

        // 创建特效
        projectileTrans = new ProjectileTrans(Caster.SubGameObject.transform.position + Vector3.up, Quaternion.Euler(180, 180, 0));
        projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
        vFXInfo = new VFXInfo();
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FireFlyNormalSkill, projectileTrans, projectileData, vFXInfo);

        // 等待动画、特效结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        yield return new WaitForSeconds(0.15f);

        // 回到起始位置
        animator.transform.localPosition = Vector3.zero;
    }
}
