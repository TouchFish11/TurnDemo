using System;
using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Quest.Config
{
    /// <summary>
    /// 任务条件配置
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public abstract class QuestConditionConfig
    {
        [JsonProperty] protected EQuestConditionType _questConditionType;

        protected QuestConditionConfig(EQuestConditionType questConditionType)
        {
            _questConditionType = questConditionType;
        }
    }
}
