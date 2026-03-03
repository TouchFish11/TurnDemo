using System;
using Core.Serialize.Binary;
using Core.Service;
using Newtonsoft.Json;

namespace GameHotUpdate.Activity.Data
{
    /// <summary>
    /// 单个活动数据
    /// 存储用户活动相关数据
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ActivityData : IActivityData
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
        public bool IsComplete => isComplete;

        /// <summary>
        /// 当前进度，每次需+=1即可
        /// 内部会自动根据进度判断是否完成
        /// </summary>
        public int CurrentPro
        {
            get => currentPro;
            set
            {
                currentPro = value;
                CheckOver();
            }
        }

        /// <summary>
        /// 检查是否完成
        /// </summary>
        private void CheckOver()
        {
            var activityInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic[activityId];
            isComplete = activityInfo.f_maxPro == currentPro;
        }
    }
}
