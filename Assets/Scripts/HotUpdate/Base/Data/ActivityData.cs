using System;
using Core.DI;
using Core.Serialize.Binary;
using Newtonsoft.Json;

namespace HotUpdate.Base.Data
{
    /// <summary>
    /// 单个活动数据
    /// 存储用户活动相关数据
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ActivityData
    {
        [JsonProperty] protected int activityId;
        [JsonProperty] protected bool isComplete;
        [JsonProperty] protected int currentPro;
        
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityId
        {
            get => activityId;
            set => activityId = value;
        }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete
        {
            get => isComplete;
            set => isComplete = value;
        }

        /// <summary>
        /// 当前进度，每次需+=1即可，需判断是否完成
        /// </summary>
        public int CurrentPro
        {
            get => currentPro;
            set => currentPro = value;
        }

        public event Action<ActivityData> OnDataChanged;
    }
}
