using System.Collections;
using Core.Config;
using Core.Reflection;
using Core.Service;
using Game.Battle.Damage;
using Game.Battle.Status;
using Game.Battle.Status.Enum;
using Game.VFX;
using GameHotUpdate.Status;
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
            foreach (var target in projectileData.targets)
            {
                foreach (int id in statusIds)
                {
                    var status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().GetStatus(id);
                    status.InitStatus(projectileData.caster, target, id);
                    if (status.StatusProperty.StatusInfo.f_statusType == (byte)E_StatusType.Negative)
                    {
                        target.GetComponent<StatusComponent>().AddStatus(status);
                    }
                }
                
                damageCalcManager.CalcSkillDamage(projectileData.caster, target, projectileData.skill.SkillInfo, out DamageResult result);
                target.TakeDamage(result);

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
