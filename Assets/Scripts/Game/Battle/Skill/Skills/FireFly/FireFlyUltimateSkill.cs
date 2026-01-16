using Framework;
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

    public FireFlyUltimateSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {

    }

    protected override void OnPreUltimateCast(IBattleContext context)
    {
        base.OnPreUltimateCast(context);

        // 播放预备动画：玩家终结技pose、终结技动画
        Caster.GetComponent<BattleAnimationComponent>().SetUltimatePose();

        projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
        projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
        vFXInfo = new VFXInfo();
        // 生成特效
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FireFlyUltimatePose, projectileTrans, projectileData, vFXInfo);
    }

    protected override IEnumerator OnUltimateCast(IBattleContext context)
    {
        // 移除Pose特效
        ServiceLocator.Get<IVFXManager>().RemoveVFX(vFXInfo);

        // 传送到主目标身前
        Vector3 targetPos = MainTarget.GameObject.transform.position;
        Caster.GameObject.transform.position = targetPos - Vector3.forward;

        yield return _waitForSeconds0_25;

        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为终结技动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(ultimateAttackState));

        projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
        projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position + Vector3.up * 0.9f, Quaternion.identity);
        // 生成特效
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FireFlyUltimateSkill, projectileTrans, projectileData, vFXInfo);
        // 等待动画结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f);

        // 回到起始位置
        targetPos = BattlePoint.Instance.GetPlayerTransByIndex(Caster.EntityPosIndex).position;
        Caster.GameObject.transform.position = targetPos;

        yield return _waitForSeconds0_25;
    }
}
