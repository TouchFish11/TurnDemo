using Newtonsoft.Json;

namespace HotUpdate.Config.Quest.Condition
{
    /// <summary>
    /// 任务击杀条件
    /// </summary>
    public class KillCondition : QuestCondition
    {
        // 击杀的目标ID
        [JsonProperty] public int targetId;
        // 击杀次数
        [JsonProperty] public int count;
    }
}
