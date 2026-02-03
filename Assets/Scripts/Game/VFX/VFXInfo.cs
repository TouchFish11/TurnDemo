using Core.Pool;
using UnityEngine;

namespace Game.VFX
{
    /// <summary>
    /// VFX信息
    /// </summary>
    public class VFXInfo : IPoolData
    {
        /// <summary>
        /// ��Ч����ϵͳ
        /// �ⲿ����Ҫ���ã�������VFX����Զ���ֵ
        /// </summary>
        public ParticleSystem ParticleSystem { get; set; }

        /// <summary>
        /// �Ƿ�ֹͣ
        /// �ⲿ�ɿ����޸�,true���Ƴ�VFX
        /// </summary>
        public bool IsStop { get; set; }

        /// <summary>
        /// �Ƿ���
        /// �ⲿ�ɻ�ȡ�������޸�
        /// </summary>
        public bool IsAlive { get; set; } = true;

        public void ResetData()
        {
            ParticleSystem = null;
            IsStop = false;
            IsAlive = true;
        }
    }
}
