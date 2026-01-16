using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// 瞬时弹射物
/// </summary>
public abstract class InstantProjectile : Projectile
{
    protected override void OnInit()
    {
        this.StartCoroutine(PlayingVFX());
    }

    protected override void Trigger()
    {
        foreach (IBattleEntityObject target in projectileData.targets)
        {
            damageCalcManager.CalcSkillDamage(projectileData.caster, target, projectileData.skill.SkillInfo, out DamageResult result);
            target.TakeDamage(result);

            // 碰撞特效
            ProjectileTrans projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
            vFXInfo = new VFXInfo();
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_BlueHit, projectileTrans, default, vFXInfo);

            projectileData.skill.RecoverEnergy();
        }
    }

    protected virtual IEnumerator PlayingVFX()
    {
        float nowTime = 0;
        int index = 0;
        while (particleSystem.IsAlive() && index < dmgTimes.Length)
        {
            nowTime += Time.deltaTime;
            if (nowTime >= dmgTimes[index])
            {
                Trigger();
                index++;
            }
            yield return null;
        }
    }
}
