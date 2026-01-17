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
[ComponentId(nameof(InteractComponent))]
public class InteractComponent : BaseComponent
{
    // 缓存当前可交互的对象
    private readonly List<IInteractable> interactables = new List<IInteractable>();
    [Header("交互配置")]
    // 交互按键
    public Key interactKey = Key.F;
    // 当前正在交互的对象
    private IInteractable currentInteractable;

    public override void Init(IEntityObject entityObject)
    {
        // 对话结束事件监听
        ServiceLocator.Get<IDialogueManager>().OnDialogueEnd += QuitInteract;
        // 交互触发事件监听
        this.EntityObject.GetComponent<InputComponent>().OnIniteract += OnIniteract;
    }

    /// <summary>
    /// 交互事件回调
    /// </summary>
    private void OnIniteract()
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
            // TODO：暂时取第一个对象，之后提供主动切换选择功能
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
        EventCenter.Instance.TriggerEvent(new InteractEvent() { Interactables = interactables });
    }

    /// <summary>
    /// 移除交互
    /// </summary>
    /// <param name="interactable"></param>
    public void RemoveInteract(IInteractable interactable)
    {
        interactables.Remove(interactable);
        EventCenter.Instance.TriggerEvent(new InteractEvent() { Interactables = interactables });
    }

    /// <summary>
    /// 退出交互
    /// </summary>
    private void QuitInteract()
    {
        currentInteractable = null;
    }


}
