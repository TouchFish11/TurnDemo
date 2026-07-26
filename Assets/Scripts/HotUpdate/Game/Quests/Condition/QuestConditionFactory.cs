using System;
using System.Collections.Generic;
using System.Reflection;
using Core.DI;
using Core.HotUpdate;
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
        private readonly Dictionary<EQuestConditionType, Func<QuestConditionConfig, IQuestCondition>> _conditions = new();

        private QuestConditionFactory(IHotUpdateManager hotUpdateManager)
        {
            foreach (var hotAssembly in hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in hotAssembly.GetTypes())
                {
                    if(!typeof(IQuestCondition).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
                        continue;

                    var conditionTypeIdAttribute = type.GetCustomAttribute<ConditionTypeIdAttribute>();
                    if (conditionTypeIdAttribute != null)
                    {
                        _conditions.Add(conditionTypeIdAttribute.ConditionType, config => (IQuestCondition)DIContainer.Create(type, config));
                    }
                }
            }
        }

        /// <summary>
        /// 根据枚举创建对应任务条件类，新增类型需要Register新实例，未找到类型返回null
        /// </summary>
        /// <param name="conditionType"></param>
        /// <param name="conditionConfig"></param>
        /// <returns></returns>
        public IQuestCondition CreateCondition(EQuestConditionType conditionType, QuestConditionConfig conditionConfig)
        {
            return _conditions.TryGetValue(conditionType, out var condition) ? condition(conditionConfig) : null;
        }
    }
}
