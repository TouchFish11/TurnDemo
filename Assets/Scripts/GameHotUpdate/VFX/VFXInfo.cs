using Core.Pool;
using UnityEngine;

namespace GameHotUpdate.VFX
{
    /// <summary>
    /// 视觉特效（VFX）信息类
    /// 用于管理粒子系统的状态和回收复用逻辑
    /// </summary>
    public class VFXInfo : IPoolData
    {
        /// <summary>
        /// 粒子系统组件
        /// 外部需要赋值，用于控制VFX的播放/停止等行为
        /// </summary>
        public ParticleSystem ParticleSystem { get; set; }

        /// <summary>
        /// 是否停止
        /// 外部可读写，设为true时会触发VFX的销毁/回收逻辑
        /// </summary>
        public bool IsStop { get; set; }

        /// <summary>
        /// 是否存活
        /// 外部仅可读，不可直接修改
        /// </summary>
        public bool IsAlive { get; set; } = true;

        /// <summary>
        /// 重置数据（池化复用接口实现）
        /// 回收时重置所有状态为初始值
        /// </summary>
        public void ResetData()
        {
            ParticleSystem = null;
            IsStop = false;
            IsAlive = true;
        }
    }
}