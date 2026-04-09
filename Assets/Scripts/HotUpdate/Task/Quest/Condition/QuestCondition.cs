using System;
using HotUpdate.Config.Quest.Config;
using HotUpdate.Core.Task;

namespace HotUpdate.Task.Quest.Condition
{
    /// <summary>
    /// 任务条件类，定义任务条件配置，将条件执行逻辑和数据分离；提供启用/禁用条件方法，启用时监听关系的事件，若满足
    /// 条件则调用InvokeOnComplete方法更新任务节点数据，提供Dispose供任务节点调用销毁
    /// </summary>
    public abstract class QuestCondition<T> : IQuestCondition, IDisposable where T : QuestConditionConfig
    {
        protected readonly T conditionConfig;
        
        public event Action<int> OnProgressChanged;

        protected QuestCondition(T questConditionConfig)
        {
            conditionConfig = questConditionConfig;
        }

        /// <summary>
        /// 启用条件，监听相关类型的事件
        /// </summary>
        public abstract void Enable();
        
        /// <summary>
        /// 禁用条件，取消监听相关类型的事件
        /// </summary>
        public abstract void Disable();

        /// <summary>
        /// 更新进度，调用OnProgressChanged回调
        /// </summary>
        /// <param name="delta"></param>
        protected void UpdateProgress(int delta)
        {
            OnProgressChanged?.Invoke(delta);
        }
        
        public void Dispose()
        {
            Disable();
            OnProgressChanged = null;
        }
    }
}
