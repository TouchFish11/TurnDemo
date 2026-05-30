using System.Collections.Generic;
using Core.Collection;
using HotUpdate.Base.Collection;
using HotUpdate.Common.Config.Quest;
using Newtonsoft.Json;

namespace HotUpdate.Game.Quests
{
    /// <summary>
    /// 任务集合
    /// </summary>
    public class QuestCollection : Collection<int, QuestData>, IQuestCollection
    {
        // 玩家同时激活（接取）的多个任务缓存，支持多任务接取
        [JsonProperty] private Dictionary<int, QuestData> _activeQuestDatas = new();
        
        /// <summary>
        /// 尝试获取正在追踪的任务
        /// </summary>
        /// <returns></returns>
        public bool TryGetTrackQuest(out QuestData data)
        {
            foreach (var questData in Values)
            {
                if (!questData.IsTracking) continue;
                data = questData;
                return true;
            }
            data = null;
            return false;
        }

        public List<QuestData> GetQuestDatas()
        {
            return new List<QuestData>(Values);
        }

        public void AddQuestData(QuestData data)
        {
            keyToValueMap.Add(data.QuestId, data);
        }
    }
}
