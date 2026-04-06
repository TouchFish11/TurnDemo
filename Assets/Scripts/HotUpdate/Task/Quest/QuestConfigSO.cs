using Core.SO;
using UnityEngine;

namespace HotUpdate.Task.Quest
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
            target = questConfig;
        }
    }
}
