using System.Threading.Tasks;
using UnityEngine;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动接口
    /// </summary>
    public interface IActivity
    {
        /// <summary>
        /// 活动游戏对象
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// 活动唯一ID
        /// </summary>
        int ActivityId { get; }

        /// <summary>
        /// 仅在首次创建时执行
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activityInfo"></param>
        /// <param name="contentHandler"></param>
        Task Init(int activityId, ActivityInfo activityInfo, IActivityContentHandler contentHandler);

        /// <summary>
        /// 首次创建时或每次激活时执行
        /// </summary>
        /// <returns></returns>
        Task Show();
        
        /// <summary>
        /// 每次隐藏失活时执行
        /// </summary>
        /// <returns></returns>
        Task Hide();
        
        /// <summary>
        /// 销毁对象，销毁前会先调用Hide
        /// </summary>
        /// <returns></returns>
        Task Destroy();
    }
}
