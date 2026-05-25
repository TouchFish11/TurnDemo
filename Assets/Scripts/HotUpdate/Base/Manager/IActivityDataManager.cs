using HotUpdate.Base.Collection;
using HotUpdate.Base.Data;

namespace HotUpdate.Base.Manager
{
    public interface IActivityDataManager : IDataManager
    {
        /// <summary>
        /// 活动数据集合
        /// </summary>
        IActivityDataCollection ActivityDataCollection { get; }
        
        /// <summary>
        /// 尝试获取活动数据
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activityData"></param>
        /// <returns></returns>
        bool TryGetData(int activityId, out ActivityData activityData);
    }
}
