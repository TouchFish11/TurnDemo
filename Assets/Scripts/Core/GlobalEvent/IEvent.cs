
using Core.Pool;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件接口
    /// </summary>
    public interface IEvent : IPoolData
    {
        /// <summary>
        /// 重置事件
        /// </summary>
        void ResetEvent();
    }
}
