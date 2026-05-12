using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Quest.Config
{
    /// <summary>
    /// 击杀条件配置
    /// </summary>
    public class KillConditionConfig : QuestConditionConfig
    {
        // 击杀的目标ID
        [JsonProperty] public int targetId;
        // 击杀次数
        [JsonProperty] public int count;

        public KillConditionConfig(EQuestConditionType questConditionType) : base(questConditionType)
        {
            
        }
    }
}
