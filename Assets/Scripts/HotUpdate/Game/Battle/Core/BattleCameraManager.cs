using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.Serialize.Binary;
using HotUpdate.Base.Manager;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.Object;
using UnityEngine;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗相机管理器
    /// </summary>
    public class BattleCameraManager : IBattleCameraManager, IDisposable
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private ObjectSpawner _objectSpawner;
        
        private readonly IMonoAdapter _monoAdapter;
        private readonly IBattleInputHandler _battleInputHandler;
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
        
        public BattleCameraManager(IBattleInputHandler battleInputHandler, 
            IMonoAdapter monoAdapter, IBattleManager battleManager)
        {
            battleInputHandler.OnDrag += OnDrag;
            battleInputHandler.OnRebound += OnRebound;
            
            monoAdapter.AddUpdateListener(OnUpdate);
            battleManager.GetContext().GetEventBus().AddListener<BattleOverEvent>(OnBattleOverEvent);
            
            _battleInputHandler = battleInputHandler;
            _monoAdapter = monoAdapter;
        }

        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot)
        {
            if(CurrentActiveCamera)
            {
                _objectSpawner.Release(CurrentActiveCamera);
                CurrentActiveCamera = null;
            }
            
            CurrentActiveCamera = await _objectSpawner.SpawnAsync<Camera>(AssetKeys.BattleCamera, cameraTrans);
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
                _objectSpawner.Release(CurrentActiveCamera);
                CurrentActiveCamera = null;
            }
            
            CurrentActiveCamera = await _objectSpawner.SpawnAsync<Camera>(AssetKeys.BattleCamera, cameraTrans);
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
                CurrentActiveCamera.transform.localRotation = Quaternion.Slerp(CurrentActiveCamera.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
            }
            // 限制最大范围，对于单位四元数的角度可以这样处理
            else
            {
                // 应用旋转
                CurrentActiveCamera.transform.localRotation = Quaternion.Slerp(CurrentActiveCamera.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
            }
        }

        /// <summary>
        /// 更新当前相机旋转基准
        /// </summary>
        public void UpdateBaseRotation()
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

        public UnityEngine.Object RayCast(int layerMask)
        {
            // 从鼠标屏幕位置发射射线，检测对应层级的战斗对象
            if (Physics.Raycast(CurrentActiveCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, 500, layerMask))
            {
                // 获取射线命中对象挂载的战斗对象组件
                var currentMainTarget = hitInfo.collider.GetComponent<BattleObject>();
                if (currentMainTarget)
                {
                    global::Core.Log.Logger.Log($"选中技能目标：{currentMainTarget.name}");
                    return currentMainTarget;
                }
                global::Core.Log.Logger.LogWarning("射线命中对象未挂载BattleObject组件");
            }
            
            return null;
        }

        /// <summary>
        /// 战斗结束事件回调
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private void OnBattleOverEvent(BattleOverEvent quitBattleEvent)
        {
            _battleInputHandler.OnDrag -= OnDrag;
            _battleInputHandler.OnRebound -= OnRebound;
            _monoAdapter.RemoveUpdateListener(OnUpdate);
        }

        public void Dispose()
        {
            _objectSpawner.Release(CurrentActiveCamera);
            CurrentActiveCamera = null;
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}
