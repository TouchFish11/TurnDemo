using System.Collections;
using Core.Config;
using Core.Service;
using Game.Battle.Damage;
using Game.Battle.Objects;
using Game.VFX;
using UnityEngine;

namespace GameHotUpdate.Battle.Projectile
{
    /// <summary>
    /// ˲ʱ������
    /// </summary>
    public abstract class InstantProjectile : Projectile
    {
        protected override void OnInit()
        {
            StartCoroutine(PlayingVFX());
        }

        protected override void Trigger()
        {
            foreach (IBattleEntityObject target in projectileData.targets)
            {
                damageCalcManager.CalcSkillDamage(projectileData.caster, target, projectileData.skill.SkillInfo, out DamageResult result);
                target.TryTakeDamage(result);

                // ��ײ��Ч
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
}
