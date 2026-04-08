using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;

namespace HotUpdate.Task.Quest.Condition
{
    /// <summary>
    /// 任务收集条件
    /// </summary>
    public class CollectCondition : QuestCondition<CollectConditionConfig>
    {
        public CollectCondition(CollectConditionConfig questConditionConfig) : base(questConditionConfig)
        {
            
        }

        public override void Enable(QuestNodeData questNodeData)
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}
