using System;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务节点配置
    /// </summary>
    [Serializable]
    public class QuestNodeConfig
    {
        // 当前节点的最大进度，达到最大进度则视为完成
        public int maxProgress;
        // 当前节点的完成条件类型，通过运行时工厂创建具体类
        public EQuestConditionType conditionType;
    }
}
