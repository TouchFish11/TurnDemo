using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 交互触发器
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractTrigger : MonoBehaviour
{
    // 获取当前对象的交互逻辑（实现IInteractable的组件）
    private IInteractable interactable;

    private void Awake()
    {
        interactable = this.GetComponent<IInteractable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractComponent interactComponent = other.GetComponent<IEntityObject>().GetComponent<InteractComponent>();
        if (interactComponent)
        {
            interactComponent.AddInteract(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractComponent interactComponent = other.GetComponent<IEntityObject>().GetComponent<InteractComponent>();
        if (interactComponent)
        {
            interactComponent.RemoveInteract(interactable);
        }
    }
}
