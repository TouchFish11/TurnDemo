using Framework;
using System.Collections.Generic;

/// <summary>
/// 交互事件
/// </summary>
public class InteractEvent : Event
{
    /// <summary>
    /// 交互对象列表
    /// </summary>
    public List<IInteractable> Interactables { get; set; }
}
