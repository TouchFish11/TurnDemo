using System;
using HotUpdate.Common.Config.Quest;

namespace HotUpdate.Common.Quest
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
        
        /// <summary>
        /// 任务节点未接取默认ID
        /// </summary>
        public const int QUEST_INACTIVE_NODE_ID = -1;

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
