using System;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Condition;

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
        /// <param name="conditionType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static QuestCondition CreateCondition(EQuestConditionType conditionType)
        {
            return conditionType switch
            {
                EQuestConditionType.Talk => new TalkCondition(),
                EQuestConditionType.Kill => new KillCondition(),
                EQuestConditionType.Collect => new CollectCondition(),
                _ => throw new ArgumentOutOfRangeException(nameof(conditionType), conditionType, null)
            };
        }
    }
}
