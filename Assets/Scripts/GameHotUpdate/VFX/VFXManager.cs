using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Singleton;
using Game.VFX;
using GameHotUpdate.Battle.Projectile;
using UnityEngine;

namespace GameHotUpdate.VFX
{
    /// <summary>
    /// �Ӿ�Ч��������
    /// </summary>
    public class VFXManager : SingletonBase<VFXManager>, IVFXManager
    {
        // ������Ч�б�
        private readonly List<VFXInfo> _activeVfxs = new List<VFXInfo>();

        private VFXManager()
        {
            ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
        }

        private void OnUpdate()
        {
            for (var i = _activeVfxs.Count - 1; i >= 0; i--)
            {
                // ��־ֹͣ���ǲ�����϶�Ҫ�Ƴ�
                if (_activeVfxs[i].IsStop || !_activeVfxs[i].ParticleSystem.IsAlive())
                {
                    _activeVfxs[i].IsAlive = false;
                    _activeVfxs[i].ParticleSystem.Stop();
                    ServiceLocator.Get<IPoolManager>().PushObj(_activeVfxs[i].ParticleSystem.gameObject);
                    _activeVfxs.RemoveAt(i);
                }
            }
        }

        // 通过泛型加载对应脚本
        public async void CreateVFX(string vfxName, ProjectileTrans projectileTrans, ProjectileData data, VFXInfo vFXInfo)
        {
            var vfxObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(EAssetBundleType.VFX, vfxName);
            vfxObj.transform.SetParent(projectileTrans.Parent, projectileTrans.WorldPositionStays);
            if (projectileTrans.Parent)
            {
                vfxObj.transform.SetLocalPositionAndRotation(projectileTrans.LocalPos, projectileTrans.Rotation);
            }
            else
            {
                vfxObj.transform.SetPositionAndRotation(projectileTrans.WorldPos, projectileTrans.Rotation);
            }

            // ���ڵ�����ű����ʼ��
            if (vfxObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Init(data, vFXInfo);
            }

            // �����Ч�Ƿ���ѭ����Ч
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
