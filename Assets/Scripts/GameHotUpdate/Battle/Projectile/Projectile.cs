using Core.Service;
using Core.Utility;
using Game.Battle.Damage;
using Game.VFX;
using UnityEngine;

namespace GameHotUpdate.Battle.Projectile
{
    /// <summary>
    /// ���������
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public abstract class Projectile : MonoBehaviour
    {
        protected VFXInfo vFXInfo;
        protected ProjectileData projectileData;
        protected new ParticleSystem particleSystem;
        protected IDamageCalcManager damageCalcManager;
        protected float[] dmgTimes;
        // buffId����
        protected int[] statusIds;
        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
            damageCalcManager = ServiceLocator.Get<IDamageCalcManager>();
        }

        /// <summary>
        /// ��ʼ��������
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="mainTarget"></param>
        /// <param name="targets"></param>
        public virtual void Init(ProjectileData projectileData, VFXInfo vFXInfo)
        {
            this.vFXInfo = vFXInfo;
            this.projectileData = projectileData;
            statusIds = projectileData.skill != null ? TextUtility.SplitToIntArr(projectileData.skill.SkillInfo.f_statusId, 2) : new int[0];
            OnInit();
        }

        /// <summary>
        /// �ڳ�ʼ����
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// ����
        /// �����˺�����������״̬Buff
        /// </summary>
        protected abstract void Trigger();
    }
}
