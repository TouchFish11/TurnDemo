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
    // 当前正在交互的对象
    private IInteractable currentInteractable;

    protected override void Awake()
    {
        base.Awake();
        // 对话结束事件监听
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueEnd += QuitInteract;
    }

    /// <summary>
    /// 交互
    /// </summary>
    public void Initeract()
    {
        if (interactables.Count == 0)
        {
            return;
        }

        if (currentInteractable != null)
        {
            currentInteractable.OnInteract(this.EntityObject);
        }
        else
        {
            currentInteractable = interactables[0];
            currentInteractable.OnInteract(this.EntityObject);
        }
    }

    /// <summary>
    /// 添加交互
    /// </summary>
    /// <param name="interactable"></param>
    public void AddInteract(IInteractable interactable)
    {
        interactables.Add(interactable);
        EventCenter.Instance.TriggerEvent(E_EventType.E_OnInteract, interactables);
    }

    /// <summary>
    /// 移除交互
    /// </summary>
    /// <param name="interactable"></param>
    public void RemoveInteract(IInteractable interactable)
    {
        interactables.Remove(interactable);
        EventCenter.Instance.TriggerEvent(E_EventType.E_OnInteract, interactables);
    }

    /// <summary>
    /// 退出交互
    /// </summary>
    private void QuitInteract()
    {
        currentInteractable = null;
    }
}
