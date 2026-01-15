using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// À≤ ±µØ…‰ŒÔ
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
