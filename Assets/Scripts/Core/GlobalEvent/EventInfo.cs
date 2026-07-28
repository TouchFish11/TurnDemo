using System;
using Core.Pool;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件信息封装类，用于存储特定类型事件的回调方法和过滤条件
    /// </summary>
    /// <typeparam name="TEvent">事件类型约束，必须实现IEvent接口</typeparam>
    public class EventInfo<TEvent> : IEventInfo, IPoolData where TEvent : IEvent
    {
        /// <summary>
        /// 事件触发时执行的回调方法
        /// </summary>
        public Action<TEvent> CallBack { get; set; }

        /// <summary>
        /// 事件执行回调前的过滤条件
        /// 返回true则执行回调，返回false则跳过
        /// </summary>
        public Func<TEvent, bool> Filter { get; set; }
        
        public int RecursionDepth { get; private set; }
        
        /// <summary>
        /// 触发事件回调（执行前会先通过过滤条件校验）
        /// </summary>
        /// <param name="info">待处理的事件实例</param>
        public void Invoke(TEvent info)
        {
            // 先执行过滤条件，只有过滤通过才执行回调
            if (Filter == null || Filter.Invoke(info))
            {
                ++RecursionDepth;
                // 回调方法不为空时执行
                CallBack?.Invoke(info);
            }
        }

        public void ResetData()
        {
            CallBack = null;
            Filter = null;
        }
    }
}