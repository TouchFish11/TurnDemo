namespace Core.GlobalEvent.Events.ViewOperation
{
    /// <summary>
    /// 界面拖曳中事件
    /// </summary>
    public class ViewDraggingEvent : ViewOperationEvent
    {
        public float DeltaX { get; set; }
        
        public float DeltaY { get; set; }
    }
}
