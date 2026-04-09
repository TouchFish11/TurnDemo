using System;

namespace HotUpdate.Core.Task
{
    /// <summary>
    /// 任务条件接口
    /// </summary>
    public interface IQuestCondition
    {
        /// <summary>
        /// 条件进度变化时时触发的事件，传递进度，由条件决定传递多少
        /// </summary>
        event Action<int> OnProgressChanged;

        /// <summary>
        /// 启用条件，监听相关类型的事件
        /// </summary>
        void Enable();

        /// <summary>
        /// 禁用条件，取消监听相关类型的事件
        /// </summary>
        void Disable();
    }
}
