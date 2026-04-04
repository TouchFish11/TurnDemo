using HotUpdate.Core.Data;

namespace HotUpdate.Core.Activity
{
    public interface IActivityData : IData<IActivityData>
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        int ActivityId { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// 当前进度，每次需+=1即可
        /// 内部会自动根据进度判断是否完成
        /// </summary>
        int CurrentPro { get; set; }
    }
}
