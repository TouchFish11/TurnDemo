using System;
using System.Collections.Generic;
using HotUpdate.Base.Quest;
using HotUpdate.Common.Config.Quest;
using HotUpdate.Common.Config.Quest.Config;

namespace HotUpdate.Game.Quests.Condition
{
    /// <summary>
    /// 任务条件工厂
    /// </summary>
    public class QuestConditionFactory
    {
        private static readonly Dictionary<EQuestConditionType, Func<QuestConditionConfig, IQuestCondition>> _conditions = new();

        /// <summary>
        /// 注册任务条件类型
        /// </summary>
        /// <param name="conditionType"></param>
        /// <param name="condition"></param>
        public static void Register(EQuestConditionType conditionType, Func<QuestConditionConfig, IQuestCondition> condition)
        {
            _conditions.Add(conditionType, condition);
        }

        /// <summary>
        /// 根据枚举创建对应任务条件类，新增类型需要Register新实例，未找到类型返回null
        /// </summary>
        /// <param name="conditionType"></param>
        /// <param name="conditionConfig"></param>
        /// <returns></returns>
        public static IQuestCondition CreateCondition(EQuestConditionType conditionType, QuestConditionConfig conditionConfig)
        {
            return _conditions.TryGetValue(conditionType, out var condition) ? condition(conditionConfig) : null;
        }
    }
}
