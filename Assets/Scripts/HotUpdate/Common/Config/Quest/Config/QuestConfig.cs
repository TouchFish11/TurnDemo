using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Quest.Config
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
        [JsonObject(MemberSerialization.OptIn)]
        public class QuestItem
        {
            [JsonProperty] public int id;
            [JsonProperty] public EQuestType questType;
            [JsonProperty] public List<QuestNodeConfig> nodeConfigs;
        }
        
        [JsonProperty] public List<QuestItem> questItems;
    }
}
