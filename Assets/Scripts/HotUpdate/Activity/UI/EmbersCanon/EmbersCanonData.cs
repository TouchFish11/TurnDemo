using System;
using System.Collections.Generic;
using HotUpdate.Activity.Core;
using HotUpdate.Base.Activity;
using Newtonsoft.Json;

namespace HotUpdate.Activity.UI.EmbersCanon
{
    /// <summary>
    /// 余烬圣典活动数据
    /// </summary>
    [ActivityId(ActivityId = 1002)]
    [Serializable]
    public class EmbersCanonData : ActivityData
    {
        [JsonProperty] private List<EmbersCanonLevelEntryData> levels = new();

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entryData"></param>
        public void Add(EmbersCanonLevelEntryData entryData)
        {
            levels.Add(entryData);
        }
        
        /// <summary>
        /// 获取关卡数据
        /// 未找到返回null
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public EmbersCanonLevelEntryData GetLevelData(int levelId)
        {
            return levels.Find(x => x.levelId == levelId);
        }
    }

    /// <summary>
    /// 余烬圣典活动关卡条目
    /// </summary>
    [Serializable]
    public class EmbersCanonLevelEntryData
    {
        public int levelId;
        public bool isComplete;
    }
}
