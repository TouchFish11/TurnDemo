using Core.UI;
using HotUpdate.Activity.Data;
using UnityEngine;

namespace HotUpdate.Activity.Core
{
    public interface IActivity : IUiBehaviour
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
