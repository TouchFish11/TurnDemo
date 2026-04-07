using System;
using HotUpdate.Config.Quest;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务工具类
    /// </summary>
    public class QuestUtil
    {
        /// <summary>
        /// 任务节点结束ID
        /// </summary>
        public const int QUEST_NODE_END_ID = -1;

        public static string ConvetTo(EQuestType questType)
        {
            return questType switch
            {
                EQuestType.Main => "主线",
                EQuestType.Side => "支线",
                EQuestType.Friend => "同行",
                _ => throw new ArgumentOutOfRangeException(nameof(questType), questType, null)
            };
        }
    }
}
