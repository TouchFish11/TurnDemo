using Framework;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 输入组件
/// </summary>
public class InputComponent : BaseComponent
{
    // 当前输入
    private Vector3 currentInput;

    /// <summary>
    /// 输入改变事件
    /// </summary>
    public event UnityAction<Vector3> OnInputChanged;

    protected override void Awake()
    {
        base.Awake();

        MonoManager.Instance.AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 帧更新
    /// </summary>
    private void OnUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 newInput = new Vector3(h, 0, z);
        if (newInput != currentInput)
        {
            OnInputChanged?.Invoke(newInput);
            currentInput = new Vector3(h, 0, z);
        }
    }

    private void OnDestroy()
    {
        OnInputChanged = null;
        if (MonoManager.IsLIve)
        {
            MonoManager.Instance.RemoveUpdateListener(OnUpdate);
        }
    }
}
