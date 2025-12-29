using Framework;
using UnityEngine;

/// <summary>
/// 环绕式第三人称摄像机
/// </summary>
public class OrbitCameraController : SingletonMono<OrbitCameraController>, IOrbitCameraController
{
    public Transform Transform { get; private set; }

    [Header("核心配置")]
    // 绕玩家的固定半径（距离）
    public float radius = 5f;
    // 看向玩家的偏移（如头顶）
    public Vector3 lookOffset = new Vector3(0, 1.5f, 0);

    [Header("灵敏度与限制")]
    // 鼠标灵敏度
    public float mouseSensitivity = 2f;
    // 垂直视角最小角度（避免视角反转）
    public float minVerticalAngle = 20f;
    // 垂直视角最大角度（避免相机穿透地面）
    public float maxVerticalAngle = 90f;
    // 是否平滑旋转
    public bool smoothRotate = true;
    // 平滑速度
    public float smoothSpeed = 10f;

    // 玩家目标（绕该对象旋转）
    private Transform player;
    // 水平旋转角度（绕Y轴）
    private float _horizontalAngle = 0f;
    // 垂直旋转角度（绕X轴）
    private float _verticalAngle = 30f;
    // 摄像机目标位置
    private Vector3 _targetCameraPos;
    // 鼠标输入
    private Vector2 mouseInput;

    protected override void Awake()
    {
        base.Awake();

        // 注册
        ServiceLocator.Instance.Register<IOrbitCameraController>(Instance);
        MonoManager.Instance.AddUpdateListener(OnUpdate);
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueStart += OnDialogueStart;
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueEnd += OnDialogueEnd;

        Transform = this.transform;

        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        // 初始化：锁定鼠标到屏幕中心
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 初始角度（可选：读取当前摄像机角度）
        if (player != null)
        {
            Vector3 dir = transform.position - player.position;
            _horizontalAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            _verticalAngle = Mathf.Asin(dir.y / radius) * Mathf.Rad2Deg;
        }
    }

    private void OnDialogueStart()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void OnDialogueEnd()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// 设置目标
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        this.player = target;
        target.GetComponent<IEntityObject>().GetComponent<InputComponent>().OnMouseSlideChanged += OnUpdateMouse;
    }

    /// <summary>
    /// 帧更新
    /// </summary>
    private void OnUpdate()
    {
        // 鼠标输入：仅当鼠标锁定时响应（按Esc/Atl可解锁，可选）
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // 鼠标滚动
            OnMouseWheel();
            // 应用鼠标输入
            ApplyMouseInput();
        }
    }

    private void LateUpdate()
    {
        // 确保玩家不为空
        if (player == null)
        {
            return;
        }

        // 计算摄像机目标位置（球面坐标转世界坐标）
        CalculateTargetPosition();

        // 平滑移动摄像机到目标位置
        if (smoothRotate)
        {
            transform.position = Vector3.Lerp(transform.position, _targetCameraPos, Time.deltaTime * smoothSpeed);
        }
        else
        {
            transform.position = _targetCameraPos;
        }

        // 强制摄像机看向玩家（带偏移）
        transform.LookAt(player.position + lookOffset);
    }

    /// <summary>
    /// 更新鼠标输入
    /// </summary>
    /// <param name="mouseInput"></param>
    private void OnUpdateMouse(Vector2 mouseInput)
    {
        this.mouseInput = mouseInput;
    }

    /// <summary>
    /// 应用鼠标输入，计算旋转角度
    /// </summary>
    private void ApplyMouseInput()
    {
        // 获取鼠标相对移动量
        float mouseX = mouseInput.x * mouseSensitivity;
        float mouseY = mouseInput.y * mouseSensitivity;

        // 水平角度：绕Y轴旋转（左右移动鼠标）
        _horizontalAngle += mouseX;
        // 垂直角度：绕X轴旋转（上下移动鼠标），并限制范围
        _verticalAngle -= mouseY;
        _verticalAngle = Mathf.Clamp(_verticalAngle, minVerticalAngle, maxVerticalAngle);
    }

    /// <summary>
    /// 计算摄像机目标位置（核心：球面坐标转笛卡尔坐标）
    /// </summary>
    private void CalculateTargetPosition()
    {
        // 将角度转为弧度
        float horizontalRad = _horizontalAngle * Mathf.Deg2Rad;
        float verticalRad = _verticalAngle * Mathf.Deg2Rad;

        // 球面坐标公式：
        // x = 圆心x + 半径 * sin(垂直角度) * sin(水平角度)
        // y = 圆心y + 半径 * cos(垂直角度)
        // z = 圆心z + 半径 * sin(垂直角度) * cos(水平角度)
        float x = player.position.x + radius * Mathf.Sin(verticalRad) * Mathf.Sin(horizontalRad);
        float y = player.position.y + radius * Mathf.Cos(verticalRad);
        float z = player.position.z + radius * Mathf.Sin(verticalRad) * Mathf.Cos(horizontalRad);

        _targetCameraPos = new Vector3(x, y, z);
    }

    /// <summary>
    /// 动态调整半径（滚轮缩放）
    /// </summary>
    private void OnMouseWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        radius = Mathf.Clamp(radius - scroll * 2f, 2f, 10f); // 限制半径范围2-10米
    }

    private void OnDestroy()
    {
        //ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueStart -= OnDialogueStart;
        //ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueEnd -= OnDialogueEnd;
    }
}
