using Framework;

/// <summary>
/// 打开界面事件
/// </summary>
public class OpenViewEvent : IEvent
{
    /// <summary>
    /// 界面控制器名称
    /// </summary>
    public string ControllerName { get; }

    public OpenViewEvent(string controllerName)
    {
        ControllerName = controllerName;
    }

    void IEvent.ResetEvent()
    {

    }
}
