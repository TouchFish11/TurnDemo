using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Singleton;
using HotUpdate.Common;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Input;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.TargetSelect;
using HotUpdate.Core.Camera;
using UnityEngine;

namespace HotUpdate.Battle
{
    /// <summary>
    /// 战斗相机管理器
    /// </summary>
    public class BattleCameraManager : IInitializable, IBattleCameraManager
    {
        public int Priority => -1;
        private readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
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

        public Camera CurrentActiveCamera { get; private set; }
        
        public Task InitAsync()
        {
            ServiceLocator.Get<IBattleInputHandler>().OnDrag += OnDrag;
            ServiceLocator.Get<IBattleInputHandler>().OnRebound += OnRebound;
            ServiceLocator.Get<ITargetSelectManager>().OnSelectChanged += OnSelectChanged;
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<BattleOverEvent>(OnBattleOverEvent);
            return Task.CompletedTask;
        }

        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot)
        {
            if(CurrentActiveCamera)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(CurrentActiveCamera.gameObject);
                CurrentActiveCamera = null;
            }
            
            var cameraObj = await _prefabLoader.GetGameObjectAsync(AbKeyCollection.Camera, ResKeyCollection.BattleCamera, cameraTrans);
            CurrentActiveCamera = cameraObj.GetComponent<Camera>();
            CurrentActiveCamera.transform.SetLocalPositionAndRotation(localPos, localRot);
            
            // 初始化当前旋转角度为相机初始角度
            currentXAngle = 0;
            _originRot = CurrentActiveCamera.transform.localRotation;
            return CurrentActiveCamera;
        }

        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot, int mask)
        {
            if(CurrentActiveCamera)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(CurrentActiveCamera.gameObject);
                CurrentActiveCamera = null;
            }
            
            var cameraObj = await _prefabLoader.GetGameObjectAsync(AbKeyCollection.Camera, ResKeyCollection.BattleCamera, cameraTrans);
            CurrentActiveCamera = cameraObj.GetComponent<Camera>();
            CurrentActiveCamera.transform.SetLocalPositionAndRotation(localPos, localRot);
            // 设置遮罩
            SetMask(CurrentActiveCamera, mask);
            // 初始化当前旋转角度为相机初始角度
            currentXAngle = 0;
            _originRot = CurrentActiveCamera.transform.localRotation;
            return CurrentActiveCamera;
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
                CurrentActiveCamera.transform.localRotation = 
                    Quaternion.Slerp(CurrentActiveCamera.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
            }
            // 限制最大范围，对于单位四元数的角度可以这样处理
            else
            {
                // 应用旋转
                CurrentActiveCamera.transform.localRotation = 
                    Quaternion.Slerp(CurrentActiveCamera.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainTarget"></param>
        private void OnSelectChanged(IBattleEntityObject mainTarget)
        {
            // 每次切换目标后，都已当前相机的面朝向作为旋转基准
            baseRotation = CurrentActiveCamera.transform.localRotation;
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
            CurrentActiveCamera.transform.localRotation = Quaternion.Slerp(CurrentActiveCamera.transform.localRotation, baseRotation, Time.deltaTime * reboundSpeed);
            if (Quaternion.Angle(CurrentActiveCamera.transform.localRotation, baseRotation) < 0.1f)
            {
                CurrentActiveCamera.transform.localRotation = baseRotation;
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
            ServiceLocator.Get<IBattleInputHandler>().OnDrag -= OnDrag;
            ServiceLocator.Get<IBattleInputHandler>().OnRebound -= OnRebound;
            ServiceLocator.Get<ITargetSelectManager>().OnSelectChanged -= OnSelectChanged;
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
        
        protected void OnDestroy()
        {
            ServiceLocator.Get<IPoolManager>().PushObj(CurrentActiveCamera.gameObject);
            CurrentActiveCamera = null;
        }
    }
}
