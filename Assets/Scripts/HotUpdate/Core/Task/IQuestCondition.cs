using System;
using HotUpdate.Config.Quest;

namespace HotUpdate.Core.Task
{
    /// <summary>
    /// 任务条件接口
    /// </summary>
    public interface IQuestCondition
    {
        /// <summary>
        /// 条件完成时触发的事件，触发后自动置空
        /// </summary>
        event Action OnComplete;

        /// <summary>
        /// 启用条件，监听相关类型的事件
        /// </summary>
        /// <param name="questNodeData"></param>
        void Enable(QuestNodeData questNodeData);

        /// <summary>
        /// 禁用条件，取消监听相关类型的事件
        /// </summary>
        void Disable();
    }
}
