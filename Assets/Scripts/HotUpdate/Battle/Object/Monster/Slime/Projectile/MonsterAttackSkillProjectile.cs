using System.Collections;
using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Projectile;
using HotUpdate.Battle.Status;
using HotUpdate.Common;
using HotUpdate.Core.Battle.Status;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Object.Monster.Slime.Projectile
{
    /// <summary>
    /// 怪物技能攻击弹射物
    /// </summary>
    public class MonsterAttackSkillProjectile : TrajectProjectile
    {
        private const float moveSpeed = 50f;
        private const float dmgDis = 1f;

        protected sealed override IEnumerator PlayingVFX()
        {
            while (Vector3.Distance(transform.position, projectileData.mainTarget.GameObject.transform.position) > dmgDis)
            {
                transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
                yield return null;
            }

            AddStatusOnTrigger();
            ApplyEffectOnTrigger();
            CreateVFXOnTrigger();
            HandleTiming();
            vFXInfo.IsStop = true;
        }

        protected override void AddStatusOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                foreach (var statusId in statusIds)
                {
                    // 获取状态实例
                    var status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().
                        GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                }
            }
        }

        protected override void ApplyEffectOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                damageCalcManager.CalcSkillDamage(projectileData.caster, target, projectileData.skill.SkillInfo, out var result);
                target.TakeDamage(result);
            }
        }

        protected override async void CreateVFXOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                var vfxInfo = new VFXInfo();
                await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_MonsterHit, projectileTrans, default, vfxInfo);
            }
        }

        protected override void HandleTiming()
        {
            
        }
    }
}
