using Framework;

/// <summary>
/// 场景加载进度事件
/// </summary>
public class SceneLoadingProgressEvent : Event
{
    /// <summary>
    /// 加载进度
    /// 范围0~1
    /// </summary>
    public float Progress { get; set; }
}
