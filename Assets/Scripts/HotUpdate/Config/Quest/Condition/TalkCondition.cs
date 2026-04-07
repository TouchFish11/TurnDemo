using Newtonsoft.Json;

namespace HotUpdate.Config.Quest.Condition
{
    /// <summary>
    /// 对话条件
    /// </summary>
    public class TalkCondition : QuestCondition
    {
        // 目标NPCID
        [JsonProperty] public int targetNpcId;
    }
}
