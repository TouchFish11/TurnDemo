using System;
using Newtonsoft.Json;

namespace HotUpdate.Config.Quest
{
    /// <summary>
    /// 任务条件类
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class QuestCondition : IDisposable
    {
        [JsonProperty] private EQuestConditionType _questConditionType;
        
        public event Action OnComplete;
        
        // 开始监听事件
        public virtual void OnStart(QuestNode node)
        {
            OnComplete?.Invoke();
            OnComplete = null;
        }
        
        // 清理事件
        public virtual void OnEnd()
        {
            
        }
        
        public void Dispose()
        {
            OnComplete = null;
        }
    }
}
