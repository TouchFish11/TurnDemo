using System;
using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Common.Config.ExcelInfo.Container;
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
        [Inject] protected IBinaryDataManager binaryDataManager;
        
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
            var activityInfo = binaryDataManager.GetConfig<ActivityInfoContainer>(EConfigLoadType.Excel).dataDic[activityId];
            isComplete = activityInfo.f_maxPro == currentPro;
        }

        public event Action<ActivityData> OnDataChanged;
    }
}
