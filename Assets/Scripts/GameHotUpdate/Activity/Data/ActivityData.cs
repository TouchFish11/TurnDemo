using System;
using Game.Data;
using UnityEngine;

namespace GameHotUpdate.Activity.Data
{
    /// <summary>
    /// 活动数据
    /// 存储用户活动相关数据
    /// </summary>
    [Serializable]
    public class ActivityData : IData
    {
        [SerializeField] private int activityId;
        [SerializeField] private bool isComplete;
        [SerializeField] private int currentPro;
        
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
        /// 当前进度
        /// 最大进度由配置表获取
        /// </summary>
        public int CurrentPro
        {
            get => currentPro;
            set => currentPro = value;
        }
    }
}
