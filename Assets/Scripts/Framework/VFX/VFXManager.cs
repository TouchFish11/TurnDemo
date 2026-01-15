using Game.Battle;
using System.Collections.Generic;
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

    /// <summary>
    /// 视觉效果管理器
    /// </summary>
    public class VFXManager : SingletonBase<VFXManager>, IVFXManager
    {
        // 特效对象缓存
        private Dictionary<string, List<GameObject>> _activeVfxs = new Dictionary<string, List<GameObject>>();

        private VFXManager()
        {

        }

        public void CreateVFX(string vfxName, Transform parent, ProjectileData data, bool worldPositionStays = false)
        {
            CreateVFX(vfxName, parent, Vector3.zero, Quaternion.identity, data, worldPositionStays);
        }

        public void CreateVFX(string vfxName, Vector3 worldPos, Quaternion quaternion, ProjectileData data)
        {
            CreateVFX(vfxName, null, worldPos, quaternion, data);
        }

        public async void CreateVFX(string vfxName, Transform parent, Vector3 localPos, Quaternion quaternion, ProjectileData data, bool worldPositionStays = false)
        {
            GameObject vfxObj = await ServiceLocator.Get<IPoolManager>().GetAssetBundleObjAsync(E_AssetBundleType.VFX, vfxName);
            vfxObj.transform.SetParent(parent, worldPositionStays);
            vfxObj.transform.SetLocalPositionAndRotation(localPos, quaternion);

            if (_activeVfxs.ContainsKey(vfxName))
            {
                _activeVfxs[vfxName].Add(vfxObj);
            }
            else
            {
                _activeVfxs.Add(vfxName, new List<GameObject>() { vfxObj });
            }

            // 检测特效是否播放完成
            if (vfxObj.TryGetComponent<ParticleSystem>(out var ps))
            {
                // 等待特效播放完毕
                float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax;
                ServiceLocator.Get<ITimerManager>().CreateTimer(false, (int)(totalDuration * 1000), () => RemoveActiveVFX(vfxObj));
            }

            // 存在弹射物脚本则初始化
            if (vfxObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Init(data);
            }
        }

        public void RemoveActiveVFX(GameObject vfxObj)
        {
            LogManager.Log($"VFX被移除：{vfxObj}");
            if (_activeVfxs.TryGetValue(vfxObj.name, out var vfxObjs))
            {
                vfxObjs.Remove(vfxObj);
                ServiceLocator.Get<IPoolManager>().PushObj(vfxObj);
            }
        }

        public void RemoveVFX(string vfxName)
        {
            if (_activeVfxs.TryGetValue(vfxName, out var vfxObjs))
            {
                foreach (var vfxObj in vfxObjs)
                {
                    ServiceLocator.Get<IPoolManager>().PushObj(vfxObj);
                }
                _activeVfxs.Remove(vfxName);
            }
        }

        public void ClearVFXCache()
        {
            foreach (var vfxObjs in _activeVfxs.Values)
            {
                foreach (var vfxObj in vfxObjs)
                {
                    ServiceLocator.Get<IPoolManager>().PushObj(vfxObj);
                }
            }
            _activeVfxs.Clear();
        }
    }
}
