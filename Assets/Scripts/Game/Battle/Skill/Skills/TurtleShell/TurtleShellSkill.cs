using Framework;
using Game;
using Game.Battle;
using System.Collections;
using UnityEngine;

public class TurtleShellSkill : MonsterSkill
{
    /// <summary>
    /// 攻击
    /// 目前是怪物使用
    /// </summary>
    public string Attack { get; } = "Attack";

    public TurtleShellSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {
        Caster.GetComponentInChildren<AnimationTrigger>().OnAttack += OnAttack;
    }

    private void OnAttack(int skillId)
    {
        if (skillId != SkillInfo.f_id)
        {
            return;
        }

        projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);

        Vector3 mainTarget = MainTarget.GameObject.transform.position;
        Vector3 realTarget = new Vector3(mainTarget.x, 0, mainTarget.z);
        Vector3 caster = Caster.GameObject.transform.position;
        Vector3 realCaster = new Vector3(caster.x, 0, caster.z);

        projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position + Vector3.forward, Quaternion.LookRotation(realTarget - realCaster));
        // 生成特效
        ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_MonsterAttackSkill, projectileTrans, projectileData, vFXInfo);
        StatusAddStrategy?.ToAdd(Caster, AllTargets, statusIds);
    }

    protected override void OnPreCast(IBattleContext context)
    {
        base.OnPreCast(context);
        vFXInfo = new VFXInfo();
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        yield return new WaitForSeconds(0.1f);
        yield return AnimationPlayManager.Instance.PlayAnimation(Caster, (E_AnimationType)SkillInfo.f_animationType, AnimationComponent.Skill_Layer_Name, Attack);
        yield return new WaitUntil(() => !vFXInfo.IsAlive);
        // 优化表现
        yield return new WaitForSeconds(0.8f);
    }
}
