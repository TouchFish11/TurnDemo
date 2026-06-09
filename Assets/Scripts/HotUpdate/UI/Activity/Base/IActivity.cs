using System.Threading.Tasks;
using Core.UI;
using HotUpdate.Common.Config.ExcelInfo.Info;
using UnityEngine;

namespace HotUpdate.UI.Activity.Base
{
    public interface IActivity : IUiBehaviour
    {
        /// <summary>
        /// 活动游戏对象
        /// </summary>
        GameObject GameObject { get; }

        int ActivityId { get; }

        /// <summary>
        /// 初始化活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activityInfo"></param>
        /// <param name="contentHandler"></param>
        Task Init(int activityId, ActivityInfo activityInfo, IActivityContentHandler contentHandler);
    }
}
