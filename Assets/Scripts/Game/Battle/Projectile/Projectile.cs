using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 弹射物基类
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public abstract class Projectile : MonoBehaviour
    {
        protected ProjectileData projectileData;
        protected new ParticleSystem particleSystem;
        protected IDamageCalcManager damageCalcManager;
        protected float[] dmgTimes;
        // buffId数组
        protected int[] statusIds;
        private void Awake()
        {
            particleSystem = this.GetComponent<ParticleSystem>();
            damageCalcManager = ServiceLocator.Get<IDamageCalcManager>();
        }

        /// <summary>
        /// 初始化弹射物
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="mainTarget"></param>
        /// <param name="targets"></param>
        public virtual void Init(ProjectileData projectileData)
        {
            this.projectileData = projectileData;
            statusIds = TextUtility.SplitToIntArr(projectileData.skill.SkillInfo.f_statusId, 2);
            OnInit();
        }

        /// <summary>
        /// 在初始化后
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// 触发
        /// 用于伤害触发、添加状态Buff
        /// </summary>
        protected abstract void Trigger();
    }
}
