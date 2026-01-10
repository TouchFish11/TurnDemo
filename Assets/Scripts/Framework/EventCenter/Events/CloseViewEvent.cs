using Framework;

/// <summary>
/// 关闭界面事件
/// </summary>
public class CloseViewEvent : IEvent
{
    /// <summary>
    /// 界面控制器名称
    /// </summary>
    public string ControllerName { get; }

    public CloseViewEvent(string controllerName)
    {
        ControllerName = controllerName;
    }

    void IEvent.ResetEvent()
    {

    }
}
