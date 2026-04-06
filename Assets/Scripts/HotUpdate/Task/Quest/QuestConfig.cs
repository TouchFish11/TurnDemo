using System;
using System.Collections.Generic;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务配置，任务项集合
    /// </summary>
    [Serializable]
    public class QuestConfig
    {
        /// <summary>
        /// 任务项
        /// </summary>
        [Serializable]
        public class QuestItem
        {
            public int id;
            public string name;
            public string description;
            public EQuestType questType;
            public List<QuestNodeConfig> nodeConfigs;
        }
        
        public List<QuestItem> questItems;
    }
}
