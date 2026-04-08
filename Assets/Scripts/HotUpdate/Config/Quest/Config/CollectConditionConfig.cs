using Newtonsoft.Json;

namespace HotUpdate.Config.Quest.Config
{
    /// <summary>
    /// 收集条件配置
    /// </summary>
    public class CollectConditionConfig : QuestConditionConfig
    {
        // 收集目标物品ID，可以用字典表示收集多个
        [JsonProperty] public int targetItemId;
        // 收集数量
        [JsonProperty] public int count;

        public CollectConditionConfig(EQuestConditionType questConditionType) : base(questConditionType)
        {

        }
    }
}
