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
    private ProjectileData projectileData;

    public FireFlyBattleSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {

    }

    protected override void OnPreCast(IBattleContext context)
    {
        base.OnPreCast(context);
        projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为战技动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(battleAttackState));
        // 生成特效
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FireFlyBattleSkill, MainTarget.GameObject.transform.position, Quaternion.LookRotation(-Caster.GameObject.transform.right), projectileData);
        // 等待动画结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f);
    }
}
