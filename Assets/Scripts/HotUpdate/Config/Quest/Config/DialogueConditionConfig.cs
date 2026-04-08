using Newtonsoft.Json;

namespace HotUpdate.Config.Quest.Config
{
    /// <summary>
    /// 对话条件配置
    /// </summary>
    public class DialogueConditionConfig : QuestConditionConfig
    {
        // 目标实体ID
        [JsonProperty] public int targetNpcId;

        public DialogueConditionConfig(EQuestConditionType questConditionType) : base(questConditionType)
        {

        }
    }
}
