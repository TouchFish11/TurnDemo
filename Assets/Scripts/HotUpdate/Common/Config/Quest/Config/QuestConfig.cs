using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Quest.Config
{
    /// <summary>
    /// 任务配置，所有任务的集合，任务项集合
    /// </summary>
    [Serializable]
    public class QuestConfig
    {
        /// <summary>
        /// 任务项，单个具体任务
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
