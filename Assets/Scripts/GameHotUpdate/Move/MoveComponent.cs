using Core.Components;
using Game.Battle.Camera;
using GameHotUpdate.Cameras;
using GameHotUpdate.Components;
using GameHotUpdate.Input;
using UnityEngine;

namespace GameHotUpdate.Move
{
    /// <summary>
    /// 移动组件
    /// 负责控制游戏实体的移动、旋转逻辑，响应输入指令并结合相机视角计算移动方向
    /// </summary>
    [ComponentId(typeof(MoveComponent))]
    [RequireComponent(typeof(CharacterControllerComponent))]
    public class MoveComponent : BaseComponent
    {
        // 移动基础速度（单位/秒）
        [SerializeField] private float speed = 7;
        // 旋转插值速度（控制转向平滑度）
        [SerializeField] private float rotateSpeed = 15;
        // 旋转阈值（角度）：小于该值时直接对齐目标旋转，停止插值旋转
        [SerializeField] private float rotateThreshold = 5f;
        // 角色控制器组件（用于处理Unity内置的角色移动碰撞等逻辑）
        private CharacterController characterController;
        // 输入方向（由InputComponent传入，x:左右，z:前后，y:无意义）
        private Vector3 inputDir;
        // 最终移动方向（结合相机视角转换后的世界空间方向）
        private Vector3 moveDir;
        // 主相机控制器（用于获取相机视角，计算相对移动方向）
        private IOrbitCameraController mainCamera;
        // 移动开关：控制是否允许执行移动/旋转逻辑
        private bool canMove;

        /// <summary>
        /// 组件初始化方法
        /// </summary>
        /// <param name="entityObject">当前挂载的实体对象</param>
        public override void Init(IEntityObject entityObject)
        {
            // 获取主相机单例并将当前实体设为相机跟随目标
            mainCamera = OrbitCameraController.Instance;
            mainCamera.SetTarget(transform);

            // 获取角色控制器组件（封装Unity原生CharacterController）
            characterController = EntityObject.GetComponent<CharacterControllerComponent>().CharacterController;
            // 订阅输入组件的输入变更事件，实时更新输入方向
            EntityObject.GetComponent<InputComponent>().OnKeyInputChanged += OnUpdateInputDir;
            // 初始化时启用移动功能
            Enable();
        }

        /// <summary>
        /// 启用移动功能
        /// </summary>
        public void Enable()
        {
            canMove = true;
        }

        /// <summary>
        /// 禁用移动功能
        /// 同时重置移动方向，防止禁用后仍有残留移动逻辑
        /// </summary>
        public void Disable()
        {
            // 重置移动方向，停止移动
            moveDir = Vector3.zero;
            canMove = false;
            // 注：此处预留“转向NPC”逻辑扩展点，暂未实现
        }

        /// <summary>
        /// 输入方向更新回调方法
        /// 由InputComponent的OnKeyInputChanged事件触发
        /// </summary>
        /// <param name="dir">新的输入方向（x:左右轴，z:前后轴）</param>
        private void OnUpdateInputDir(Vector3 dir)
        {
            inputDir = dir;
        }

        /// <summary>
        /// 外部设置移动开关状态
        /// </summary>
        /// <param name="canMove">是否允许移动</param>
        public void SetMoveFlag(bool canMove)
        {
            this.canMove = canMove;
        }

        /// <summary>
        /// 帧更新逻辑
        /// 核心移动/旋转逻辑的入口，仅在允许移动时执行
        /// </summary>
        private void Update()
        {
            // 移动开关关闭时，直接返回不执行后续逻辑
            if (!canMove)
            {
                return;
            }

            // 计算最终移动方向（结合相机视角）
            CalcDirection();
            // 根据移动方向更新实体旋转
            UpdateRotate();
            // 执行实体移动
            UpdateMove();
        }

        /// <summary>
        /// 计算最终移动方向
        /// 将输入方向转换为基于相机视角的世界空间方向
        /// </summary>
        private void CalcDirection()
        {
            // 输入方向幅值过小（近似无输入），重置移动方向并返回
            if (inputDir.magnitude < 0.1f)
            {
                moveDir = Vector3.zero;
                return;
            }

            // 提取相机的前向、右向向量，并忽略Y轴（仅保留水平平面）
            var camForward = mainCamera.Transform.forward;
            var camRight = mainCamera.Transform.right;
            camForward.y = 0; // 消除垂直方向影响，仅在水平平面移动
            camRight.y = 0;   // 消除垂直方向影响，仅在水平平面移动
            camForward.Normalize(); // 归一化，确保向量长度为1，不影响移动速度
            camRight.Normalize();   // 归一化，确保向量长度为1，不影响移动速度

            // 组合相机前向/右向向量，得到世界空间下的移动方向
            // 输入映射：W键（z+）=相机前向，S键（z-）=相机后向，A键（x-）=相机左向，D键（x+）=相机右向
            moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        }

        /// <summary>
        /// 更新实体旋转
        /// 使实体朝向与移动方向一致，带阈值的平滑旋转逻辑
        /// </summary>
        private void UpdateRotate()
        {
            // 无移动方向时，不执行旋转逻辑
            if (moveDir == Vector3.zero)
            {
                return;
            }

            // 计算目标旋转：朝向移动方向（仅绕Y轴旋转）
            var targetRot = Quaternion.LookRotation(moveDir);
            // 计算当前旋转与目标旋转的夹角（仅Y轴）
            var angleDiff = Quaternion.Angle(transform.rotation, targetRot);
            
            // 夹角大于阈值时，平滑插值旋转（避免微小平移导致频繁旋转）
            if (angleDiff > rotateThreshold)
            {
                // 球形插值旋转，保证旋转平滑且速度可控
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
            // 夹角小于等于阈值时，直接对齐目标旋转（消除微小偏移，避免抖动）
            else
            {
                transform.rotation = targetRot;
            }
        }

        /// <summary>
        /// 执行实体移动
        /// 通过CharacterController实现碰撞检测的移动逻辑
        /// </summary>
        private void UpdateMove()
        {
            // 基于最终移动方向、速度和帧时间，执行移动（Time.deltaTime保证帧率无关）
            characterController.Move(speed * Time.deltaTime * moveDir);
        }
    }
}