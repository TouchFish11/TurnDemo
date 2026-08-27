using System;

namespace Core.GlobalEvent.Events.ViewOperation
{
    public abstract class ViewOperationEvent : Event
    {
        public Type UIView { get; set; }
    }
}
