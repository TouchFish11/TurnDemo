using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using UnityEngine;

namespace HotUpdate.Game.VFX
{
    /// <summary>
    /// 视觉特效（VFX）管理器
    /// 负责VFX的创建、更新、移除、缓存清理等核心逻辑，基于对象池管理VFX资源
    /// </summary>
    public class VFXManager : IVFXManager
    {
        [Inject] private ObjectSpawner _objectSpawner;
        private readonly IMonoAdapter _monoAdapter;
        // 存储当前活跃的VFX信息
        private readonly Dictionary<VFXInfo, GameObject> _activeVfxInfos = new();
        // 待移除的vfx信息缓存
        private readonly List<VFXInfo> _removedVfxInfos = new();
        
        public VFXManager(IMonoAdapter monoAdapter)
        {
            // 注册帧更新监听，用于检测VFX状态
            monoAdapter.AddUpdateListener(OnUpdate);
            _monoAdapter = monoAdapter;
        }

        /// <summary>
        /// 帧更新回调，检测并回收非活跃的VFX
        /// </summary>
        private void OnUpdate()
        {
            foreach (var (vfxInfo, vfxObj) in _activeVfxInfos)
            {
                // 已停止或粒子系统非存活状态，回收至对象池
                if (vfxInfo.IsStop || !vfxInfo.ParticleSystem.IsAlive())
                {
                    vfxInfo.IsAlive = false;
                    vfxInfo.ParticleSystem.Stop();
                    // 将VFX对象归还至对象池
                    _objectSpawner.Release(vfxObj);
                    // 放入待删除列表
                    _removedVfxInfos.Add(vfxInfo);
                }
            }
            
            foreach (var removedVfxInfo in _removedVfxInfos)
            {
                _activeVfxInfos.Remove(removedVfxInfo);
            }
            _removedVfxInfos.Clear();
        }

        /// <summary>
        /// 创建投射物相关的VFX（异步）
        /// </summary>
        /// <param name="vfxName">VFX资源名称</param>
        /// <param name="projectileTrans">投射物变换信息</param>
        /// <param name="data">投射物数据</param>
        /// <param name="vFXInfo">VFX信息载体</param>
        public async Task<IProjectile> CreateVFX(string vfxName, ProjectileTrans projectileTrans, ProjectileData data, VFXInfo vFXInfo)
        {
            // 异步获取VFX资源
            var vfxObj = await _objectSpawner.SpawnAsync<GameObject>(vfxName, projectileTrans.Parent, worldSpace:projectileTrans.WorldPositionStays);
            // 根据父物体是否存在，设置VFX的位置和旋转
            if (projectileTrans.Parent)
            {
                vfxObj.transform.SetLocalPositionAndRotation(projectileTrans.LocalPos, projectileTrans.Rotation);
            }
            else
            {
                vfxObj.transform.SetPositionAndRotation(projectileTrans.WorldPos, projectileTrans.Rotation);
            }

            // 如果VFX挂载了投射物组件，初始化投射物数据
            if (vfxObj.TryGetComponent<IProjectile>(out var projectile))
            {
                _monoAdapter.StartCoroutine(projectile.InitToStart(data, vFXInfo));
            }

            // 如果包含粒子系统，记录到活跃列表
            if (vfxObj.TryGetComponent<ParticleSystem>(out var ps))
            {
                vFXInfo.ParticleSystem = ps;
                _activeVfxInfos.Add(vFXInfo, vfxObj);
            }

            return projectile;
        }

        /// <summary>
        /// 创建指定父物体/位置的VFX（异步），适用于没有IProjectile的特效
        /// </summary>
        /// <param name="vfxName">VFX资源名称</param>
        /// <param name="parent">父物体Transform</param>
        /// <param name="pos">本地位置</param>
        /// <param name="rot">旋转</param>
        /// <param name="vFXInfo">VFX信息载体</param>
        public async Task CreateVFX(string vfxName, Transform parent, Vector3 pos, Quaternion rot, VFXInfo vFXInfo)
        {
            // 异步获取VFX资源
            var vfxObj = await _objectSpawner.SpawnAsync<GameObject>(vfxName, parent,  pos, rot);
            // 如果包含粒子系统，记录到活跃列表
            if (vfxObj.TryGetComponent<ParticleSystem>(out var ps))
            {
                vFXInfo.ParticleSystem = ps;
                _activeVfxInfos.Add(vFXInfo, vfxObj);
            }
        }
        
        /// <summary>
        /// 主动移除指定VFX
        /// </summary>
        /// <param name="vFXInfo">要移除的VFX信息</param>
        public void RemoveVFX(VFXInfo vFXInfo)
        {
            // 检查是否在活跃列表中，存在则回收至对象池并移除
            if (!_activeVfxInfos.TryGetValue(vFXInfo, out var activeVfx)) 
                return;

            _objectSpawner.Release(activeVfx);
            _activeVfxInfos.Remove(vFXInfo);
        }

        /// <summary>
        /// 清理所有VFX缓存（回收全部活跃VFX至对象池）
        /// </summary>
        public void ClearVFXCache()
        {
            // 遍历所有活跃VFX，逐一回收至对象池
            foreach (var vfxObj in _activeVfxInfos.Values)
            {
                _objectSpawner.Release(vfxObj);
            }
            // 清空活跃列表
            _activeVfxInfos.Clear();
            _objectSpawner.Dispose();
        }
    }
}