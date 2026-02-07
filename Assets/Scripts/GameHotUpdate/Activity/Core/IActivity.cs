using GameHotUpdate.Activity.Data;
using UnityEngine;

namespace GameHotUpdate.Activity.Core
{
    public interface IActivity
    {
        /// <summary>
        /// 活动游戏对象
        /// </summary>
        GameObject GameObject { get; }
        
        /// <summary>
        /// 活动数据
        /// </summary>
        ActivityData ActivityData { get; }

        /// <summary>
        /// 初始化活动
        /// </summary>
        /// <param name="activityData"></param>
        /// <param name="activityInfo"></param>
        void Init(ActivityData activityData, ActivityInfo activityInfo);
    }
}
