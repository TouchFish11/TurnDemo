using System;
using Core.SO;
using UnityEngine;

namespace HotUpdate.Common.Config.Quest.Config
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
                    if (questNodeConfig.conditionConfig == null || 
                        questNodeConfig.conditionConfig is DialogueConditionConfig && questNodeConfig.conditionType != EQuestConditionType.Talk || 
                        questNodeConfig.conditionConfig is KillConditionConfig && questNodeConfig.conditionType != EQuestConditionType.Kill || 
                        questNodeConfig.conditionConfig is CollectConditionConfig && questNodeConfig.conditionType != EQuestConditionType.Collect)
                    {
                        questNodeConfig.conditionConfig = questNodeConfig.conditionType switch
                        {
                            EQuestConditionType.Talk => new DialogueConditionConfig(questNodeConfig.conditionType),
                            EQuestConditionType.Kill => new KillConditionConfig(questNodeConfig.conditionType),
                            EQuestConditionType.Collect => new CollectConditionConfig(questNodeConfig.conditionType),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                }
            }
            
            target = questConfig;
        }

        protected override void OnAwake()
        {
            
        }
    }
}
