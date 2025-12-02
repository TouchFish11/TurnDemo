using Framework;
using Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 交互组件
/// </summary>
public class InteractComponent : BaseComponent
{
    // 缓存当前可交互的对象
    private readonly List<IInteractable> interactables = new List<IInteractable>();

    [Header("交互配置")]
    // 交互按键
    public Key interactKey = Key.F;
    // 交互提示UI（拖入UGUI Text）
    public Text promptText;
    // 是否正在交互
    private bool isInteracting;

    protected override void Awake()
    {
        base.Awake();
        MonoManager.Instance.AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 交互
    /// </summary>
    public void Initeract()
    {
        isInteracting = true;
        interactables[0].OnInteract(this.EntityObject);
    }

    /// <summary>
    /// 帧更新
    /// </summary>
    private void OnUpdate()
    {
        // F键交互
        if (Keyboard.current.fKey.wasPressedThisFrame && !isInteracting && interactables.Count > 0)
        {
            Initeract();
        }
    }

    public void AddInteract(IInteractable interactable)
    {
        interactables.Add(interactable);
        EventCenter.Instance.TriggerEvent(E_EventType.E_OnInteract, interactables);
    }

    public void RemoveInteract(IInteractable interactable)
    {
        interactables.Remove(interactable);
        EventCenter.Instance.TriggerEvent(E_EventType.E_OnInteract, interactables);
    }

    /// <summary>
    /// 退出交互
    /// </summary>
    public void QuitInteract()
    {
        isInteracting = false;
    }

    public override void Destroy()
    {
        base.Destroy();
        MonoManager.Instance.RemoveUpdateListener(OnUpdate);
    }
}
