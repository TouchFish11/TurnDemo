using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 史莱姆和甲壳虫怪物攻击弹射物
/// </summary>
public class MonsterAttackSkillProjectile : TrajectProjectile
{
    private float moveSpeed = 50f;
    private float dmgDis = 1f;

    protected override void OnInit()
    {
        this.StartCoroutine(PlayingVFX());
    }

    protected override void Trigger()
    {
        // 处理伤害
        foreach (IBattleEntityObject target in projectileData.targets)
        {
            damageCalcManager.CalcSkillDamage(projectileData.caster, target, projectileData.skill.SkillInfo, out DamageResult result);
            target.TakeDamage(result);
            // 碰撞特效
            ProjectileTrans projectileTrans = new ProjectileTrans(target.GameObject.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            VFXInfo vFXInfo = new VFXInfo();
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_MonsterHit, projectileTrans, default, vFXInfo);
        }

        vFXInfo.IsStop = true;
    }

    protected override IEnumerator PlayingVFX()
    {
        while (Vector3.Distance(this.transform.position, projectileData.mainTarget.GameObject.transform.position) > dmgDis)
        {
            this.transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
            yield return null;
        }

        Trigger();
    }
}
