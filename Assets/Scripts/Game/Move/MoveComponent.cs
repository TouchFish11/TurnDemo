using Framework;
using Game;
using UnityEngine;

/// <summary>
/// 移动组件
/// </summary>
public class MoveComponent : BaseComponent
{
    // 移动速度
    [SerializeField] private float speed = 7;
    // 旋转速度
    [SerializeField] private float rotateSpeed = 15;
    // 旋转阈值（角度，小于该值则停止旋转）
    [SerializeField] private float rotateThreshold = 5f;
    // 角色控制器组件
    private CharacterController characterController;
    // 输入方向
    private Vector3 inputDir;
    // 最终移动方向（世界坐标系）
    private Vector3 moveDir;
    // 主摄像机引用
    private Camera mainCamera;
    // 能否移动
    private bool canMove;

    protected override void Awake()
    {
        base.Awake();

        mainCamera = OrbitCameraController.Instance.GetComponent<Camera>();
        characterController = this.EntityObject.GetComponent<CharacterController>();
        this.EntityObject.GetComponent<InputComponent>().OnKeyInputChanged += OnUpdateInputDir;
    }

    public void Enable()
    {
        canMove = true;
    }

    public void Disable()
    {
        // 停止移动
        moveDir = Vector3.zero;
        canMove = false;
        // 转向NPC
    }

    /// <summary>
    /// 更新输入方向
    /// </summary>
    /// <param name="dir"></param>
    private void OnUpdateInputDir(Vector3 dir)
    {
        this.inputDir = dir;
    }

    /// <summary>
    /// 设置移动标识
    /// </summary>
    /// <param name="canMove"></param>
    public void SetMoveFlag(bool canMove)
    {
        this.canMove = canMove;
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        CalcDirection();
        UpdateRotate();
        UpdateMove();
    }

    /// <summary>
    /// 计算方向
    /// </summary>
    private void CalcDirection()
    {
        // 无输入时，停止移动和旋转
        if (inputDir.magnitude < 0.1f)
        {
            moveDir = Vector3.zero;
            return;
        }

        // 将输入方向转换为，摄像机视角下的世界方向
        // 摄像机的前/右方向（忽略Y轴，保持水平，单位化）
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 计算目标移动方向（W=摄像机前，A=摄像机左，D=摄像机右，S=摄像机后）
        moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
    }

    /// <summary>
    /// 更新旋转
    /// </summary>
    private void UpdateRotate()
    {
        if (moveDir == Vector3.zero)
        {
            return;
        }

        // 目标方向的四元数（仅Y轴旋转）
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        // 计算当前前方向与目标方向的夹角（仅Y轴）
        float angleDiff = Quaternion.Angle(transform.rotation, targetRot);
        // 夹角大于阈值，继续旋转
        if (angleDiff > rotateThreshold)
        {
            // 平滑旋转到目标方向
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
        // 否则停止旋转，保持朝向
        else
        {
            // 强制对齐目标方向（避免微小偏移）
            transform.rotation = targetRot;
        }
    }

    /// <summary>
    /// 更新移动
    /// </summary>
    private void UpdateMove()
    {
        // 角色移动逻辑
        characterController.Move(speed * Time.deltaTime * moveDir);
    }
}