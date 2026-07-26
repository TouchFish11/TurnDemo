using HotUpdate.Common.Config.Quest;
using HotUpdate.Common.Config.Quest.Config;

namespace HotUpdate.Game.Quests.Condition
{
    /// <summary>
    /// 任务击杀条件
    /// </summary>
    [ConditionTypeId(EQuestConditionType.Kill)]
    public class KillCondition : QuestCondition<KillConditionConfig>
    {
        public KillCondition(QuestConditionConfig questConditionConfig) : base(questConditionConfig as KillConditionConfig)
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
