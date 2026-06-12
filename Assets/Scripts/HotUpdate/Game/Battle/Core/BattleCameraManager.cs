using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.Serialize.Binary;
using HotUpdate.Base;
using HotUpdate.Base.Manager;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.TargetSelect;
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
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IBattleManager _battleManager;
        
        private BattleCoordinator _battleCoordinator;
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
        // 当前选中的技能ID（用于释放技能时匹配技能配置）
        private int skillId;
        
        public Camera CurrentActiveCamera { get; private set; }
        
        public BattleCameraManager(BattleCoordinator battleCoordinator, IBattleInputHandler battleInputHandler)
        {
            _battleCoordinator = battleCoordinator;
            _battleInputHandler = battleInputHandler;
            battleInputHandler.OnDrag += OnDrag;
            battleInputHandler.OnRebound += OnRebound;
            battleInputHandler.OnClick += OnClick;
            
            _targetSelectManager.OnSelectChanged += OnSelectChanged;
            _monoAdapter.AddUpdateListener(OnUpdate);
            _battleManager.GetContext().GetEventBus().AddListener<BattleOverEvent>(OnBattleOverEvent);
            // 从战斗管理器事件总线订阅技能选择事件，接收选中的技能ID
            _battleManager.GetContext().GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
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
        /// 目标选择切换事件回调
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

        private void OnClick()
        {
            // 根据选中的技能ID获取技能配置信息
            var skillInfo = _binaryDataManager.GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
            // 将技能范围类型转换为技能目标类型（友方/敌方）
            var targetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;

            // 根据技能目标类型设置射线检测的层级掩码（只检测对应层级的对象）
            int layerMask;
            switch (targetType)
            {
                case E_SkillTargetType.Friend:
                    // 检测玩家对象层级
                    layerMask = LayerGeter.GetRoleBitLayer();
                    break;
                case E_SkillTargetType.Enemy:
                    // 检测怪物对象层级
                    layerMask = LayerGeter.GetMonsterBitLayer();
                    break;
                case E_SkillTargetType.None:
                default:
                    global::Core.Log.Logger.LogWarning($"未处理的技能目标类型：{targetType}");
                    return;
            }
                
            // 从鼠标屏幕位置发射射线，检测对应层级的战斗对象
            if (Physics.Raycast(CurrentActiveCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, 500, layerMask))
            {
                // 获取射线命中对象挂载的战斗对象组件
                var currentMainTarget = hitInfo.collider.GetComponent<BattleObject>();
                if (currentMainTarget)
                {
                    _battleCoordinator.SelectedEntity(currentMainTarget);
                    global::Core.Log.Logger.Log($"选中技能目标：{currentMainTarget.name}");
                }
                else
                {
                    global::Core.Log.Logger.LogWarning("射线命中对象未挂载BattleObject组件");
                }
            }
        }

        /// <summary>
        /// 战斗结束事件回调
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private void OnBattleOverEvent(BattleOverEvent quitBattleEvent)
        {
            _battleInputHandler.OnDrag -= OnDrag;
            _battleInputHandler.OnRebound -= OnRebound;
            DIContainer.GetInstance<ITargetSelectManager>().OnSelectChanged -= OnSelectChanged;
            _monoAdapter.RemoveUpdateListener(OnUpdate);
        }
        
        /// <summary>
        /// 技能选择事件回调
        /// 接收并缓存选中的技能ID
        /// </summary>
        /// <param name="selectSkillEvent">技能选择事件数据</param>
        private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        {
            skillId = selectSkillEvent.SkillId;
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
