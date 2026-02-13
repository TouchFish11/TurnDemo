using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Singleton;
using Game.Battle.Input;
using Game.Battle.Objects;
using Game.Battle.TargetSelect;
using Game.Objects;
using UnityEngine;

namespace GameHotUpdate.Cameras
{
    /// <summary>
    /// 战斗相机管理器
    /// </summary>
    public class BattleCameraManager : SingletonAutoMono<BattleCameraManager>, IBattleCameraManager
    {
        // X轴旋转角度限制
        private const float minXAngle = -3f;
        private const float maxXAngle = 3f;
        // 旋转灵敏度
        private const float rotateSpeed = 1f;
        // 回弹速度
        private const float reboundSpeed = 8f;
        // 当前相机旋转角度
        private float currentXAngle;
        
        public Camera CurrentActiveCamera { get; private set; }
        
        public GameObject GameObject => this.gameObject;
        
        private void Awake()
        {
            ServiceLocator.Get<IBattleInputHandler>().OnDrag += OnDrag;
            ServiceLocator.Get<IBattleInputHandler>().OnRebound += OnRebound;

            ServiceLocator.Get<ITargetSelectManager>().OnSelectChanged += OnSelectChanged;
            
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot)
        {
            if(CurrentActiveCamera)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(CurrentActiveCamera.gameObject);
                CurrentActiveCamera = null;
            }
            
            var cameraObj = await ServiceLocator.Get<IObjectBuilder>()
                .GetGameobject(EAssetBundleType.Camera, ResKeyCollection.BattleCamera, cameraTrans);
            CurrentActiveCamera = cameraObj.GetComponent<Camera>();
            CurrentActiveCamera.transform.SetLocalPositionAndRotation(localPos, localRot);
            
            // 初始化当前旋转角度为相机初始角度
            currentXAngle = 0;
            return CurrentActiveCamera;
        }

        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot, int mask)
        {
            if(CurrentActiveCamera)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(CurrentActiveCamera.gameObject);
                CurrentActiveCamera = null;
            }
            
            var cameraObj = await ServiceLocator.Get<IObjectBuilder>()
                .GetGameobject(EAssetBundleType.Camera, ResKeyCollection.BattleCamera, cameraTrans);
            CurrentActiveCamera = cameraObj.GetComponent<Camera>();
            CurrentActiveCamera.transform.SetLocalPositionAndRotation(localPos, localRot);
            // 设置遮罩
            SetMask(CurrentActiveCamera, mask);
            // 更新相机角度
            
            // 初始化当前旋转角度为相机初始角度
            currentXAngle = 0;
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

        private bool isRebound;
        private Quaternion currentFwdRotation;  // 当前相机的面朝向四元数
        private Quaternion baseRotation; // 基准旋转（看向目标的初始旋转）
        private float lastDeltaX;
        
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
            // 限制最大范围
            currentXAngle = Mathf.Clamp(currentXAngle, minXAngle, maxXAngle);
            var targetRot = Quaternion.Euler(0f, currentXAngle, 0f);
            // 应用旋转
            CurrentActiveCamera.transform.localRotation = Quaternion.Slerp(CurrentActiveCamera.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
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

        protected override void OnDestroy()
        {
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
            base.OnDestroy();
        }
    }
}
