using Game.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public struct ProjectileData
    {
        public IBattleEntityObject caster;
        public IBattleEntityObject mainTarget;
        public List<IBattleEntityObject> targets;
        public ISkill skill;

        public ProjectileData(IBattleEntityObject caster, IBattleEntityObject mainTarget, List<IBattleEntityObject> targets, ISkill skill)
        {
            this.caster = caster;
            this.mainTarget = mainTarget;
            this.targets = targets;
            this.skill = skill;
        }
    }

    public readonly struct ProjectileTrans
    {
        public Transform Parent { get; }
        public Vector3 WorldPos { get; }
        public Vector3 LocalPos { get; }
        public Quaternion Rotation { get; }
        public bool WorldPositionStays { get; }

        public ProjectileTrans(Transform parent, bool worldPositionStays) : this()
        {
            Parent = parent;
            WorldPositionStays = worldPositionStays;
        }

        public ProjectileTrans(Vector3 worldPos, Quaternion rotation) : this()
        {
            WorldPos = worldPos;
            Rotation = rotation;
        }

        public ProjectileTrans(Transform parent, Vector3 localPos, Quaternion rotation, bool worldPositionStays) : this()
        {
            Parent = parent;
            LocalPos = localPos;
            Rotation = rotation;
            WorldPositionStays = worldPositionStays;
        }
    }
    
    /// <summary>
    /// VFX信息
    /// </summary>
    public class VFXInfo : IPoolData
    {
        /// <summary>
        /// 特效粒子系统
        /// 外部不需要设置，创建完VFX后会自动赋值
        /// </summary>
        public ParticleSystem ParticleSystem { get; set; }

        /// <summary>
        /// 是否停止
        /// 外部可控制修改,true则移除VFX
        /// </summary>
        public bool IsStop { get; set; } = false;

        /// <summary>
        /// 是否存活
        /// 外部可获取但不可修改
        /// </summary>
        public bool IsAlive { get; set; } = true;

        public void ResetData()
        {
            ParticleSystem = null;
            IsStop = false;
            IsAlive = true;
        }
    }

    /// <summary>
    /// 视觉效果管理器
    /// </summary>
    public class VFXManager : SingletonBase<VFXManager>, IVFXManager
    {
        // 激活特效列表
        private List<VFXInfo> _activeVfxs = new List<VFXInfo>();

        private VFXManager()
        {
            ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
        }

        private void OnUpdate()
        {
            for (int i = _activeVfxs.Count - 1; i >= 0; i--)
            {
                // 标志停止或是播放完毕都要移除
                if (_activeVfxs[i].IsStop || !_activeVfxs[i].ParticleSystem.IsAlive())
                {
                    _activeVfxs[i].IsAlive = false;
                    _activeVfxs[i].ParticleSystem.Stop();
                    ServiceLocator.Get<IPoolManager>().PushObj(_activeVfxs[i].ParticleSystem.gameObject);
                    _activeVfxs.RemoveAt(i);
                }
            }
        }

        public async void CreateVFX(string vfxName, ProjectileTrans projectileTrans, ProjectileData data, VFXInfo vFXInfo)
        {
            GameObject vfxObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(E_AssetBundleType.VFX, vfxName);
            vfxObj.transform.SetParent(projectileTrans.Parent, projectileTrans.WorldPositionStays);
            if (projectileTrans.Parent != null)
            {
                vfxObj.transform.SetLocalPositionAndRotation(projectileTrans.LocalPos, projectileTrans.Rotation);
            }
            else
            {
                vfxObj.transform.SetPositionAndRotation(projectileTrans.WorldPos, projectileTrans.Rotation);
            }

            // 存在弹射物脚本则初始化
            if (vfxObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Init(data, vFXInfo);
            }

            // 检测特效是否是循环特效
            if (vfxObj.TryGetComponent<ParticleSystem>(out var ps))
            {
                vFXInfo.ParticleSystem = ps;
                _activeVfxs.Add(vFXInfo);
            }
        }

        public void RemoveVFX(VFXInfo vFXInfo)
        {
            if (_activeVfxs.Contains(vFXInfo))
            {
                ServiceLocator.Get<IPoolManager>().PushObj(vFXInfo.ParticleSystem.gameObject);
                _activeVfxs.Remove(vFXInfo);
            }
        }

        public void ClearVFXCache()
        {
            foreach (var vFXInfo in _activeVfxs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(vFXInfo.ParticleSystem.gameObject);
            }
            _activeVfxs.Clear();
        }
    }
}
