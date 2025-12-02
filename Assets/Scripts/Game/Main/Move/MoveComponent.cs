using Framework;
using Game;
using UnityEngine;

/// <summary>
/// 移动组件
/// </summary>
public class MoveComponent : BaseComponent
{
    // 当前方向
    private Vector3 currentDir;
    // 移动速度
    [SerializeField] private float speed = 10;
    // 角色控制器组件
    private CharacterController characterController;

    protected override void Awake()
    {
        base.Awake();

        characterController = this.EntityObject.AddComponent<CharacterController>();
        this.EntityObject.GetComponent<InputComponent>().OnInputChanged += OnUpdateDir;
    }

    /// <summary>
    /// 更新方向
    /// </summary>
    /// <param name="dir"></param>
    private void OnUpdateDir(Vector3 dir)
    {
        this.currentDir = dir.normalized;
    }

    private void Update()
    {
        characterController.Move(speed * Time.deltaTime * currentDir);
    }

    private void OnDestroy()
    {
        this.EntityObject.GetComponent<InputComponent>().OnInputChanged -= OnUpdateDir;
    }
}
