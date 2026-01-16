using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// Herta普攻
/// </summary>
public class HertaNormalSkill : PlayerSkill
{
    private readonly string attackState = "NormalAttack";

    public HertaNormalSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {

    }

    protected override void OnPreCast(IBattleContext context)
    {
        base.OnPreCast(context);
        projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
        projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
        vFXInfo = new VFXInfo();
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        // 播放动画
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        // 等待动画切换为普攻动画
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(attackState));
        // 生成特效
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_HertaNormalSkill, projectileTrans, projectileData, vFXInfo);
        // 等待动画结束
        yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
    }
}
