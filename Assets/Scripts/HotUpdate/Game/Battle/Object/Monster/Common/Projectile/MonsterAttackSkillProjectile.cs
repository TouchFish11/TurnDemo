using System.Collections;
using HotUpdate.Game.Battle.Projectile;
using HotUpdate.Game.Battle.Skill.Base;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.Common.Projectile
{
    /// <summary>
    /// 怪物技能攻击弹射物
    /// </summary>
    public class MonsterAttackSkillProjectile : TrajectProjectile
    {
        private const float moveSpeed = 50f;
        private const float dmgDis = 1f;

        protected sealed override IEnumerator ExecuteVFX()
        {
            while (Vector3.Distance(transform.position, projectileData.mainTarget.GameObject.transform.position) > dmgDis)
            {
                transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
                yield return null;
            }

            var result = new HitResult(true, 0);
            InvokeOnTrigger(result);
        }
    }
}
