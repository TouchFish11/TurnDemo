using System.Collections;
using Core.Config;
using Core.Service;
using Game.VFX;
using UnityEngine;

namespace GameHotUpdate.Battle.Projectile.Projectiles.Monster
{
    /// <summary>
    /// ʷ��ķ�ͼ׿ǳ���﹥��������
    /// </summary>
    public class MonsterAttackSkillProjectile : TrajectProjectile
    {
        private float moveSpeed = 50f;
        private float dmgDis = 1f;

        protected override void OnInit()
        {
            StartCoroutine(PlayingVFX());
        }

        protected override void Trigger()
        {
            // �����˺�
            foreach (var target in projectileData.targets)
            {
                damageCalcManager.CalcSkillDamage(projectileData.caster, target, projectileData.skill.SkillInfo, out var result);
                target.TakeDamage(result);
                // ��ײ��Ч
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                var vfxInfo = new VFXInfo();
                ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_MonsterHit, projectileTrans, default, vfxInfo);
            }

            vFXInfo.IsStop = true;
        }

        protected override IEnumerator PlayingVFX()
        {
            while (Vector3.Distance(transform.position, projectileData.mainTarget.GameObject.transform.position) > dmgDis)
            {
                transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
                yield return null;
            }

            Trigger();
        }
    }
}
