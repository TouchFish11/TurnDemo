namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务阶段
    /// </summary>
    public enum EQuestPhase : byte
    {
        /// <summary>
        /// 未接取
        /// </summary>
        NoReceive,
        
        /// <summary>
        /// 进行中，同时只能有一个任务处于该状态
        /// </summary>
        Processing,
        
        /// <summary>
        /// 已完成
        /// </summary>
        Complete,
    }
}
