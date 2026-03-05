using System;
using System.Collections.Generic;
using Core.Loader.Object;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Singleton;
using HotUpdate.Battle.Projectile;
using HotUpdate.Config;
using UnityEngine;

namespace HotUpdate.VFX
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 视觉特效（VFX）管理器
    /// 负责VFX的创建、更新、移除、缓存清理等核心逻辑，基于对象池管理VFX资源
    /// </summary>
    public class VFXManager : SingletonBase<VFXManager>, IVFXManager
    {
        private readonly IMonoAdapter _monoAdapter = ServiceLocator.Get<IMonoAdapter>();
        private readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        // 存储当前活跃的VFX信息列表
        private readonly List<VFXInfo> _activeVfxs = new();

        /// <summary>
        /// 私有构造函数（单例模式），注册Update监听
        /// </summary>
        private VFXManager()
        {
            // 注册帧更新监听，用于检测VFX状态
            _monoAdapter.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 帧更新回调，检测并回收非活跃的VFX
        /// </summary>
        private void OnUpdate()
        {
            // 倒序遍历，避免移除元素导致的索引异常
            for (var i = _activeVfxs.Count - 1; i >= 0; i--)
            {
                // 已停止或粒子系统非存活状态，回收至对象池
                if (_activeVfxs[i].IsStop || !_activeVfxs[i].ParticleSystem.IsAlive())
                {
                    _activeVfxs[i].IsAlive = false;
                    _activeVfxs[i].ParticleSystem.Stop();
                    // 将VFX对象归还至对象池
                    ServiceLocator.Get<IPoolManager>().PushObj(_activeVfxs[i].ParticleSystem.gameObject);
                    // 从活跃列表移除
                    _activeVfxs.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 创建投射物相关的VFX（异步）
        /// </summary>
        /// <param name="vfxName">VFX资源名称</param>
        /// <param name="projectileTrans">投射物变换信息</param>
        /// <param name="data">投射物数据</param>
        /// <param name="vFXInfo">VFX信息载体</param>
        public async Task CreateVFX(string vfxName, ProjectileTrans projectileTrans, ProjectileData data, VFXInfo vFXInfo)
        {
            try
            {
                // 异步获取VFX资源
                var vfxObj = await _prefabLoader.GetGameObjectAsync(AbKeyCollection.Vfx, vfxName, projectileTrans.Parent, projectileTrans.WorldPositionStays);
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
                if (vfxObj.TryGetComponent<Projectile>(out var projectile))
                {
                    projectile.Init(data, vFXInfo);
                }

                // 如果包含粒子系统，记录到活跃列表
                if (vfxObj.TryGetComponent<ParticleSystem>(out var ps))
                {
                    vFXInfo.ParticleSystem = ps;
                    _activeVfxs.Add(vFXInfo);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(VFXManager)}.{nameof(CreateVFX)}：{e.Message}，{e.StackTrace}");
            }
        }

        /// <summary>
        /// 创建指定父物体/位置的VFX（异步）
        /// </summary>
        /// <param name="vfxName">VFX资源名称</param>
        /// <param name="parent">父物体Transform</param>
        /// <param name="pos">本地位置</param>
        /// <param name="rot">旋转</param>
        /// <param name="vFXInfo">VFX信息载体</param>
        public async Task CreateVFX(string vfxName, Transform parent, Vector3 pos, Quaternion rot, VFXInfo vFXInfo)
        {
            try
            {
                // 异步获取VFX资源
                var vfxObj = await _prefabLoader.GetGameObjectAsync(AbKeyCollection.Vfx, vfxName, parent,  pos, rot);
                // 如果包含粒子系统，记录到活跃列表
                if (vfxObj.TryGetComponent<ParticleSystem>(out var ps))
                {
                    vFXInfo.ParticleSystem = ps;
                    _activeVfxs.Add(vFXInfo);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(VFXManager)}.{nameof(CreateVFX)}：{e.Message}，{e.StackTrace}");
            }
        }
        
        /// <summary>
        /// 主动移除指定VFX
        /// </summary>
        /// <param name="vFXInfo">要移除的VFX信息</param>
        public void RemoveVFX(VFXInfo vFXInfo)
        {
            // 检查是否在活跃列表中，存在则回收至对象池并移除
            if (_activeVfxs.Contains(vFXInfo))
            {
                ServiceLocator.Get<IPoolManager>().PushObj(vFXInfo.ParticleSystem.gameObject);
                _activeVfxs.Remove(vFXInfo);
            }
        }

        /// <summary>
        /// 清理所有VFX缓存（回收全部活跃VFX至对象池）
        /// </summary>
        public void ClearVFXCache()
        {
            // 遍历所有活跃VFX，逐一回收至对象池
            foreach (var vFXInfo in _activeVfxs)
            {
                _prefabLoader.CollectAsset(vFXInfo.ParticleSystem.gameObject);
                _prefabLoader.RealseAsset(AbKeyCollection.Vfx, vFXInfo.ParticleSystem.gameObject.name);
            }
            // 清空活跃列表
            _activeVfxs.Clear();
        }
    }
}