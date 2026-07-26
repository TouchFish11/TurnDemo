using HotUpdate.Common.Config.Quest;
using HotUpdate.Common.Config.Quest.Config;

namespace HotUpdate.Game.Quests.Condition
{
    /// <summary>
    /// 任务收集条件
    /// </summary>
    [ConditionTypeId(EQuestConditionType.Collect)]
    public class CollectCondition : QuestCondition<CollectConditionConfig>
    {
        public CollectCondition(QuestConditionConfig questConditionConfig) : base(questConditionConfig as CollectConditionConfig)
        {
            
        }

        public override void Enable()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}
