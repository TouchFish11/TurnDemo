
namespace Framework
{
    /// <summary>
    /// 事件接口
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// 重置事件
        /// 用于复用事件时，重置事件成员
        /// </summary>
        void ResetEvent();
    }
}
