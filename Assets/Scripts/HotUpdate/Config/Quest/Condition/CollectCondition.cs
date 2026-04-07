using Newtonsoft.Json;

namespace HotUpdate.Config.Quest.Condition
{
    /// <summary>
    /// 任务收集条件
    /// </summary>
    public class CollectCondition : QuestCondition
    {
        // 收集目标物品ID，可以用字典表示收集多个
        [JsonProperty] public int targetItemId;
        // 收集数量
        [JsonProperty] public int count;
    }
}
