using HotUpdate.Common.Config.Quest;
using HotUpdate.Common.Config.Quest.Config;

namespace HotUpdate.Game.Quests.Condition
{
    /// <summary>
    /// 任务条件工厂初始化器
    /// </summary>
    public class QuestConditionFactoryInitializer
    {
        public static void Initialize()
        {
            QuestConditionFactory.Register(EQuestConditionType.Talk, config => new TalkCondition(config as DialogueConditionConfig));
            QuestConditionFactory.Register(EQuestConditionType.Kill, config => new KillCondition(config as KillConditionConfig));
            QuestConditionFactory.Register(EQuestConditionType.Collect, config => new CollectCondition(config as CollectConditionConfig));
        }
    }
}
