using Core.Components;
using Core.Mono;
using Core.Service;
using Core.Singleton;
using HotUpdate.Core.Camera;
using HotUpdate.Core.Input;
using UnityEngine;

namespace HotUpdate.Camera
{
    /// <summary>
    /// 环绕式相机控制器（第三人称轨道相机）
    /// 核心功能：围绕目标（玩家）旋转、滚轮缩放、对话时切换光标状态
    /// </summary>
    public class OrbitCameraController : SingletonMono<OrbitCameraController>, IOrbitCameraController
    {
        private readonly IMouseManager _mouseManager = ServiceLocator.Get<IMouseManager>();
        private readonly IMonoAdapter _monoAdapter = ServiceLocator.Get<IMonoAdapter>();
        
        // 相机自身Transform缓存
        public Transform Transform { get; private set; }

        [Header("相机基础配置")]
        // 相机到目标的初始半径（控制相机与玩家距离）
        public float radius = 4f;
        // 相机看向目标时的偏移量（避免相机看向玩家脚底）
        public Vector3 lookOffset = new(0, 1.5f, 0);

        [Header("相机旋转配置")]
        // 鼠标拖动灵敏度
        public float mouseSensitivity = 0.2f;
        // 垂直旋转最小角度（限制相机向下旋转的极限）
        public float minVerticalAngle = 50f;
        // 垂直旋转最大角度（限制相机向上旋转的极限）
        public float maxVerticalAngle = 90f;
        // 是否开启平滑旋转
        public bool smoothRotate;
        // 平滑旋转速度
        public float smoothSpeed = 15f;

        // 相机跟随的目标（玩家Transform）
        private Transform player;
        // 水平旋转角度（绕Y轴）
        private float _horizontalAngle;
        // 垂直旋转角度（绕X轴）
        private float _verticalAngle = 30f;
        // 相机需要移动到的目标位置
        private Vector3 _targetCameraPos;
        // 鼠标拖动输入值
        private Vector2 mouseInput;
        
        private GameObject _gameObject;
        private EntityProperty _entityProperty;
        public IEntityObject EntityObject { get; private set; }

        /// <summary>
        /// 单例初始化（继承SingletonMono）
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // 注册帧更新监听
            _monoAdapter.AddUpdateListener(OnUpdate);
            // 缓存自身Transform
            Transform = transform;
            // 初始化光标和角度
            Init();
        }

        /// <summary>
        /// 初始化：设置光标状态 + 初始旋转角度
        /// </summary>
        private void Init()
        {
            // 如果已有目标，根据当前相机与目标的位置计算初始旋转角度
            if (player)
            {
                var dir = transform.position - player.position;
                _horizontalAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                _verticalAngle = Mathf.Asin(dir.y / radius) * Mathf.Rad2Deg;
            }
        }

        /// <summary>
        /// 设置相机跟随的目标
        /// </summary>
        /// <param name="target">玩家Transform</param>
        public void SetTarget(Transform target)
        {
            player = target;
            // 注册鼠标拖动输入监听
            target.GetComponent<IEntityObject>().GetComponent<IInputComponent>().OnMouseSlideChanged += OnUpdateMouse;
        }

        /// <summary>
        /// 帧更新逻辑（仅在光标锁定时执行：滚轮缩放 + 鼠标旋转）
        /// </summary>
        private void OnUpdate()
        {
            // 仅在游戏操作状态（光标锁定）时处理相机输入
            if (_mouseManager.LockState == CursorLockMode.Locked)
            {
                // 处理鼠标滚轮缩放
                OnMouseWheel();
                // 处理鼠标拖动旋转
                ApplyMouseInput();
            }
        }

        /// <summary>
        /// 延迟更新（保证在玩家移动后执行，避免相机位置偏移）
        /// </summary>
        private void LateUpdate()
        {
            // 无目标时直接返回
            if (!player)
            {
                return;
            }

            // 计算相机需要移动到的目标位置
            CalculateTargetPosition();

            // 平滑/直接移动到目标位置
            if (smoothRotate)
            {
                transform.position = Vector3.Lerp(transform.position, _targetCameraPos, Time.deltaTime * smoothSpeed);
            }
            else
            {
                transform.position = _targetCameraPos;
            }

            // 强制让相机看向目标（叠加偏移量，保证看向玩家头部）
            transform.LookAt(player.position + lookOffset);
        }

        /// <summary>
        /// 鼠标拖动输入更新回调
        /// </summary>
        /// <param name="mouseInput">鼠标拖动的增量值</param>
        private void OnUpdateMouse(Vector2 mouseInput)
        {
            this.mouseInput = mouseInput;
        }

        /// <summary>
        /// 应用鼠标输入：更新相机旋转角度（限制垂直角度范围）
        /// </summary>
        private void ApplyMouseInput()
        {
            // 鼠标输入 * 灵敏度 = 实际旋转增量
            float mouseX = mouseInput.x * mouseSensitivity;
            float mouseY = mouseInput.y * mouseSensitivity;

            // 水平角度累加（绕Y轴旋转）
            _horizontalAngle += mouseX;
            // 垂直角度累减（绕X轴旋转），并限制在最小/最大值之间
            _verticalAngle -= mouseY;
            _verticalAngle = Mathf.Clamp(_verticalAngle, minVerticalAngle, maxVerticalAngle);
        }

        /// <summary>
        /// 计算相机目标位置（基于球面坐标系公式）
        /// 核心公式：将角度转换为三维空间坐标，围绕目标生成相机位置
        /// </summary>
        private void CalculateTargetPosition()
        {
            // 角度转弧度
            float horizontalRad = _horizontalAngle * Mathf.Deg2Rad;
            float verticalRad = _verticalAngle * Mathf.Deg2Rad;

            // 球面坐标系转笛卡尔坐标系：
            // x = 目标x + 半径 * sin(垂直角度) * sin(水平角度)
            // y = 目标y + 半径 * cos(垂直角度)
            // z = 目标z + 半径 * sin(垂直角度) * cos(水平角度)
            float x = player.position.x + radius * Mathf.Sin(verticalRad) * Mathf.Sin(horizontalRad);
            float y = player.position.y + radius * Mathf.Cos(verticalRad);
            float z = player.position.z + radius * Mathf.Sin(verticalRad) * Mathf.Cos(horizontalRad);

            // 赋值给目标位置
            _targetCameraPos = new Vector3(x, y, z);
        }

        /// <summary>
        /// 处理鼠标滚轮：动态调整相机半径（缩放），限制范围2-10
        /// </summary>
        private void OnMouseWheel()
        {
            // 获取滚轮输入（向前为正，向后为负）
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            // 调整半径并限制范围（滚轮每滚动1单位，半径变化2f）
            radius = Mathf.Clamp(radius - scroll * 2f, 2f, 10f);
        }

        /// <summary>
        /// 销毁时移除监听（防止内存泄漏）
        /// </summary>
        private void OnDestroy()
        {
            _monoAdapter.RemoveUpdateListener(OnUpdate);
        }
        
        #region 无用接口实现（IEntityObject）
        // 以下为接口强制实现的无用代码，无实际业务逻辑
        GameObject IEntityObject.GameObject => _gameObject;
        EntityProperty IEntityObject.EntityProperty => _entityProperty;
        void IEntityObject.BaseInit(int id) { }
        T IEntityObject.GetComponent<T>() => default;
        TComponent IEntityObject.GetComponentInChildren<TComponent>() => default;
        TComponent IEntityObject.AddComponent<TComponent>() => null;
        bool IEntityObject.AddComponents(params string[] componentNames) => false;
        void IEntityObject.Destroy() { }
        #endregion
    }
}