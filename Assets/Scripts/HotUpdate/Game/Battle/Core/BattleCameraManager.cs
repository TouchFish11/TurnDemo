using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.Mono.MonoFunction;
using HotUpdate.Base;
using HotUpdate.Base.Manager;
using HotUpdate.Common;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.TargetSelect;
using UnityEngine;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗相机管理器
    /// </summary>
    public class BattleCameraManager : IBattleCameraManager, IDestroyable
    {
        [Inject] private ObjectSpawner _objectSpawner;
        
        // X轴旋转角度限制
        private const float minXAngle = -3f;
        private const float maxXAngle = 3f;
        // 旋转灵敏度
        private const float rotateSpeed = 1f;
        // 回弹速度
        private const float reboundSpeed = 8f;
        // 当前相机旋转角度
        private float currentXAngle;
        // 是否回弹
        private bool isRebound;
        // 当前相机的面朝向四元数
        private Quaternion currentFwdRotation;  
        // 基准旋转（看向目标的初始旋转）
        private Quaternion baseRotation; 
        // 上一次的滑动偏移
        private float lastDeltaX;
        // 相机起始角度
        private Quaternion _originRot;

        public PoolObject<Camera> CurrentActiveCameraPoolObject { get; private set; }

        public BattleCameraManager()
        {
            DIContainer.GetInstance<IBattleInputHandler>().OnDrag += OnDrag;
            DIContainer.GetInstance<IBattleInputHandler>().OnRebound += OnRebound;
            DIContainer.GetInstance<ITargetSelectManager>().OnSelectChanged += OnSelectChanged;
            DIContainer.GetInstance<IMonoAdapter>().AddUpdateListener(OnUpdate);
            DIContainer.GetInstance<IBattleManager>().GetContext().GetEventBus().AddListener<BattleOverEvent>(OnBattleOverEvent);
        }

        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot)
        {
            if(CurrentActiveCameraPoolObject.Obj)
            {
                CurrentActiveCameraPoolObject.Collect();
                CurrentActiveCameraPoolObject = default;
            }
            
            CurrentActiveCameraPoolObject = await _objectSpawner.SpawnAsync<Camera>(ResKeyCollection.BattleCamera, cameraTrans);
            CurrentActiveCameraPoolObject.Obj.transform.SetLocalPositionAndRotation(localPos, localRot);
            
            // 初始化当前旋转角度为相机初始角度
            currentXAngle = 0;
            _originRot = CurrentActiveCameraPoolObject.Obj.transform.localRotation;
            return CurrentActiveCameraPoolObject.Obj;
        }

        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot, int mask)
        {
            if(CurrentActiveCameraPoolObject.Obj)
            {
                CurrentActiveCameraPoolObject.Collect();
                CurrentActiveCameraPoolObject = default;
            }
            
            CurrentActiveCameraPoolObject = await _objectSpawner.SpawnAsync<Camera>(ResKeyCollection.BattleCamera, cameraTrans);
            CurrentActiveCameraPoolObject.Obj.transform.SetLocalPositionAndRotation(localPos, localRot);
            // 设置遮罩
            SetMask(CurrentActiveCameraPoolObject.Obj, mask);
            // 初始化当前旋转角度为相机初始角度
            currentXAngle = 0;
            _originRot = CurrentActiveCameraPoolObject.Obj.transform.localRotation;
            return CurrentActiveCameraPoolObject.Obj;
        }

        /// <summary>
        /// 设置遮罩
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="mask"></param>
        private static void SetMask(Camera camera, int mask)
        {
            camera.cullingMask = 0;
            camera.cullingMask = mask;
        }
        
        /// <summary>
        /// 滑动事件回调
        /// </summary>
        /// <param name="deltaX"></param>
        private void OnDrag(float deltaX)
        {
            if (Mathf.Approximately(lastDeltaX, deltaX) || isRebound)
            {
                return;
            }

            lastDeltaX = deltaX;
            currentXAngle += deltaX;
            currentXAngle = Mathf.Clamp(currentXAngle, minXAngle, maxXAngle);
            var targetRot = Quaternion.Euler(0f, currentXAngle, 0f);
            
            if (_originRot != Quaternion.identity)
            {
                // 以起始四元数为基准的偏移
                targetRot *= _originRot;
                // 应用旋转
                CurrentActiveCameraPoolObject.Obj.transform.localRotation = Quaternion.Slerp(CurrentActiveCameraPoolObject.Obj.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
            }
            // 限制最大范围，对于单位四元数的角度可以这样处理
            else
            {
                // 应用旋转
                CurrentActiveCameraPoolObject.Obj.transform.localRotation = Quaternion.Slerp(CurrentActiveCameraPoolObject.Obj.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
            }
        }
        
        /// <summary>
        /// 目标选择切换事件回调
        /// </summary>
        /// <param name="mainTarget"></param>
        private void OnSelectChanged(IBattleEntityObject mainTarget)
        {
            // 每次切换目标后，都已当前相机的面朝向作为旋转基准
            baseRotation = CurrentActiveCameraPoolObject.Obj.transform.localRotation;
        }
        
        private void OnUpdate()
        {
            if (!isRebound)
            {
                return;
            }
            
            Rebounding();
        }

        /// <summary>
        /// 是否回弹
        /// </summary>
        /// <param name="isRebound"></param>
        private void OnRebound(bool isRebound)
        {
            this.isRebound = isRebound;
        }

        /// <summary>
        /// 回弹效果
        /// </summary>
        private void Rebounding()
        {
            CurrentActiveCameraPoolObject.Obj.transform.localRotation = Quaternion.Slerp(CurrentActiveCameraPoolObject.Obj.transform.localRotation, baseRotation, Time.deltaTime * reboundSpeed);
            if (Quaternion.Angle(CurrentActiveCameraPoolObject.Obj.transform.localRotation, baseRotation) < 0.1f)
            {
                CurrentActiveCameraPoolObject.Obj.transform.localRotation = baseRotation;
                OnRebound(false);
                currentXAngle = 0;
            }
        }

        /// <summary>
        /// 战斗结束事件回调
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private void OnBattleOverEvent(BattleOverEvent quitBattleEvent)
        {
            DIContainer.GetInstance<IBattleInputHandler>().OnDrag -= OnDrag;
            DIContainer.GetInstance<IBattleInputHandler>().OnRebound -= OnRebound;
            DIContainer.GetInstance<ITargetSelectManager>().OnSelectChanged -= OnSelectChanged;
            DIContainer.GetInstance<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
        
        public void OnDestroy()
        {
            CurrentActiveCameraPoolObject.Collect();
            CurrentActiveCameraPoolObject = default;
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}
