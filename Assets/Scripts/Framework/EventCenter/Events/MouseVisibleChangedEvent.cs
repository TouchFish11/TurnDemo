using Framework;

/// <summary>
/// 鼠标可见性变化事件
/// </summary>
public class MouseVisibleChangedEvent : Event
{
    public string SourceName { get; set; }

    public bool IsVisible { get; set; }

}
