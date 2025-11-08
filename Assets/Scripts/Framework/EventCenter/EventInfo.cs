using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 不携带参数的事件信息类
    /// </summary>
    public class EventInfo : BaseEventInfo
    {
        /// <summary>
        /// 事件回调
        /// </summary>
        public event UnityAction EventCallBack;

        public EventInfo(UnityAction EventCallBack)
        {
            this.EventCallBack += EventCallBack;
        }

        /// <summary>
        /// 调用
        /// </summary>
        public void Invoke()
        {
            EventCallBack?.Invoke();
        }
    }

    /// <summary>
    /// 携带参数的事件信息类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventInfo<T> : BaseEventInfo
    {
        /// <summary>
        /// 事件回调
        /// </summary>
        public event UnityAction<T> EventCallBack;

        public EventInfo(UnityAction<T> EventCallBack)
        {
            this.EventCallBack += EventCallBack;
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="info">传递信息</param>
        public void Invoke(T info)
        {
            EventCallBack?.Invoke(info);
        }
    }
}
