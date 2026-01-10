using Framework;
using System.Collections.Generic;

/// <summary>
/// ½»»¥ÊÂ¼þ
/// </summary>
public class InteractEvent : IEvent
{
    public List<IInteractable> Interactables { get; }

    public InteractEvent(List<IInteractable> interactables)
    {
        Interactables = interactables;
    }

    void IEvent.ResetEvent()
    {

    }
}
