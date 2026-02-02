using System;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 延迟事件信息封装类
    /// 用于存储延迟事件的触发回调和执行过滤条件，提供统一的事件执行入口
    /// </summary>
    public class DelayEventInfo
    {
        /// <summary>
        /// 延迟事件触发时的回调方法
        /// 私有设置器，确保外部仅能通过初始化/特定逻辑赋值，避免随意修改
        /// </summary>
        public Action TriggerCallback { private get; set; }

        /// <summary>
        /// 延迟事件执行前的过滤条件
        /// 返回 true 表示满足执行条件，触发 TriggerCallback；返回 false 则不执行
        /// 私有设置器，确保过滤逻辑不会被外部随意篡改
        /// </summary>
        public Func<bool> Filter { private get; set; }

        /// <summary>
        /// 执行延迟事件的核心方法
        /// 先校验过滤条件，满足则触发回调
        /// </summary>
        /// <exception cref="NullReferenceException">Filter 为 null 时调用会抛出空引用异常，使用前需确保 Filter 已赋值</exception>
        public void Invoke()
        {
            // 执行过滤条件判断，仅当条件满足时触发回调
            if (Filter.Invoke())
            {
                TriggerCallback?.Invoke();
            }
        }
    }
}