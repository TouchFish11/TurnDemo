using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// Herta终结技
/// </summary>
public class HertaUltimateSkill : UltimateSkill
{
    private readonly string ultimateAttackState = "UltimateAttack";

    public HertaUltimateSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
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
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_HeartUltimatePose, projectileTrans, projectileData, vFXInfo);
    }

    protected override IEnumerator OnUltimateCast(IBattleContext context)
    {
        // 移除Pose特效
        ServiceLocator.Get<IVFXManager>().RemoveVFX(vFXInfo);
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为终结技攻击动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(ultimateAttackState));

        projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
        projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_HeartUltimateSkill, projectileTrans, projectileData, vFXInfo);
        // 等待动画、特效结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
    }
}
