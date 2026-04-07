namespace HotUpdate.Config.Quest
{
    /// <summary>
    /// 任务阶段，每个任务节点的阶段
    /// </summary>
    public enum EQuestPhase : byte
    {
        /// <summary>
        /// 未接取，存在该任务但没有追踪该节点任务，即接取后又取消接取就是这个阶段
        /// </summary>
        NoReceive,
        
        /// <summary>
        /// 进行中，处于正在追踪该节点任务，所有任务中只能有一个任务节点处于该状态
        /// </summary>
        Processing,
        
        /// <summary>
        /// 已完成，该节点任务已完成
        /// </summary>
        Complete,
    }
}
