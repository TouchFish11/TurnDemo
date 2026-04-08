using System;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;
using HotUpdate.Core.Task;

namespace HotUpdate.Task.Quest.Condition
{
    /// <summary>
    /// 任务条件工厂
    /// </summary>
    public class QuestConditionFactory
    {
        /// <summary>
        /// 根据枚举创建对应任务条件类，新增类型需要新增实例
        /// </summary>
        /// <param name="conditionType">条件类型</param>
        /// <param name="conditionConfig">条件配置</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IQuestCondition CreateCondition(EQuestConditionType conditionType, QuestConditionConfig conditionConfig)
        {
            return conditionType switch
            {
                EQuestConditionType.Talk => new TalkCondition(conditionConfig as DialogueConditionConfig),
                EQuestConditionType.Kill => new KillCondition(conditionConfig as KillConditionConfig),
                EQuestConditionType.Collect => new CollectCondition(conditionConfig as CollectConditionConfig),
                _ => throw new ArgumentOutOfRangeException(nameof(conditionType), conditionType, null)
            };
        }
    }
}
