using System;

namespace Core.EventCenter
{
    /// <summary>
    /// 事件信息封装类，用于存储特定类型事件的回调方法和过滤条件
    /// </summary>
    /// <typeparam name="TEvent">事件类型约束，必须实现IEvent接口</typeparam>
    public class EventInfo<TEvent> : BaseEventInfo where TEvent : IEvent
    {
        /// <summary>
        /// 事件触发时执行的回调方法
        /// </summary>
        public Action<TEvent> CallBack { get; }

        /// <summary>
        /// 事件执行回调前的过滤条件
        /// 返回true则执行回调，返回false则跳过
        /// </summary>
        public Func<TEvent, bool> Filter { get; }

        /// <summary>
        /// 初始化EventInfo实例
        /// </summary>
        /// <param name="callBack">事件回调方法</param>
        /// <param name="filter">事件过滤条件，可为null（null时默认返回true）</param>
        public EventInfo(Action<TEvent> callBack, Func<TEvent, bool> filter)
        {
            CallBack = callBack;
            // 过滤条件为空时，默认设置为始终返回true的委托，确保后续调用不出现空引用
            Filter = filter ?? ((evt) => true);
        }

        /// <summary>
        /// 触发事件回调（执行前会先通过过滤条件校验）
        /// </summary>
        /// <param name="info">待处理的事件实例</param>
        public void Invoke(TEvent info)
        {
            // 先执行过滤条件，只有过滤通过才执行回调
            if (Filter.Invoke(info))
            {
                // 回调方法不为空时执行
                CallBack?.Invoke(info);
            }
        }
    }
}