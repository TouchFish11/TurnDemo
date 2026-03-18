namespace Core.Mono
{
    /// <summary>
    /// 应用程序退出通知接口
    /// 需要响应应用程序退出生命周期函数的管理器对象实现此接口
    /// </summary>
    public interface IApplicationExitNotify
    {
        /// <summary>
        /// 退出优先级
        /// 数值越小越先执行
        /// </summary>
        int QuitPriority { get; }
        
        /// <summary>
        /// 应用退出时执行
        /// </summary>
        void OnAppQuit();
    }
}
