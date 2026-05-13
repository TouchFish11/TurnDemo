using Core.UI;
using HotUpdate.Common.Config.ExcelInfo.Info;
using UnityEngine;

namespace HotUpdate.Base.Activity
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
        void Init(IActivityData activityData, ActivityInfo activityInfo);
    }
}
