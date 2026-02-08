using System;
using System.Collections.Generic;
using GameHotUpdate.Activity.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace GameHotUpdate.Activity.UI.EmbersCanon
{
    /// <summary>
    /// 余烬圣典活动数据
    /// </summary>
    [Serializable]
    public class EmbersCanonData : ActivityData
    {
        [JsonProperty]
        [SerializeField] private List<EmbersCanonLevelEntry> levels = new();

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entry"></param>
        public void Add(EmbersCanonLevelEntry entry)
        {
            levels.Add(entry);
        }
        
        /// <summary>
        /// 获取关卡数据
        /// 未找到返回null
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public EmbersCanonLevelEntry GetLevelData(int levelId)
        {
            return levels.Find(x => x.levelId == levelId);
        }
    }

    /// <summary>
    /// 余烬圣典活动关卡条目
    /// </summary>
    [Serializable]
    public class EmbersCanonLevelEntry
    {
        public int levelId;
        public bool isComplete;
    }
}
