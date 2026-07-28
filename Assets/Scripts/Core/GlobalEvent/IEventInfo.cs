namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件信息接口
    /// </summary>
    internal interface IEventInfo
    {
        /// <summary>
        /// 递归深度
        /// </summary>
        int RecursionDepth { get; }
    }
}
