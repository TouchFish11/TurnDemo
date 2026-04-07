using System;
using Core.SO;
using HotUpdate.Config.Quest.Condition;
using UnityEngine;

namespace HotUpdate.Config.Quest
{
    /// <summary>
    /// 任务配置SO
    /// </summary>
    [CreateAssetMenu(fileName = "QuestConfig", menuName = "Task/QuestConfigSO")]
    public class QuestConfigSO : SOBase
    {
        public QuestConfig questConfig;

        private void OnValidate()
        {
            // 该逻辑是为了让输入的时候不会重新去new条件，避免修改其它字段值的时候导致条件被重新new覆盖，只有条件类型变化的时候才去更新条件对象
            foreach (var questConfigQuestItem in questConfig.questItems)
            {
                foreach (var questNodeConfig in questConfigQuestItem.nodeConfigs)
                {
                    if (questNodeConfig.condition == null || 
                        questNodeConfig.condition is TalkCondition && questNodeConfig.conditionType != EQuestConditionType.Talk || 
                        questNodeConfig.condition is KillCondition && questNodeConfig.conditionType != EQuestConditionType.Kill || 
                        questNodeConfig.condition is CollectCondition && questNodeConfig.conditionType != EQuestConditionType.Collect)
                    {
                        questNodeConfig.condition = questNodeConfig.conditionType switch
                        {
                            EQuestConditionType.Talk => new TalkCondition(),
                            EQuestConditionType.Kill => new KillCondition(),
                            EQuestConditionType.Collect => new CollectCondition(),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                }
            }
            
            target = questConfig;
        }
    }
}
