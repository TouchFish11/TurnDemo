using Core.Pool;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件中心模块的事件基类，所有自定义事件需继承此类
    /// </summary>
    /// <remarks>
    /// 该抽象类实现了 <see cref="IEvent"/> 接口，为所有事件提供统一的基类定义，
    /// 确保事件体系的一致性和可扩展性。
    /// </remarks>
    public abstract class Event : IEvent
    {
        /// <summary>
        /// 重置事件对象的状态，用于事件对象池复用场景
        /// </summary>
        /// <remarks>
        /// 子类可重写此方法，实现自定义的状态重置逻辑（例如清空参数、重置标记位等）。
        /// 基类实现为空，仅提供默认行为。
        /// </remarks>
        public virtual void ResetEvent()
        {
            // 基类默认无重置逻辑，由子类按需重写
        }

        void IPoolData.ResetData()
        {
            ResetEvent();
        }
    }
}