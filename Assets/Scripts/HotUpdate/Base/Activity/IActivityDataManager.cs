using HotUpdate.Base.Data;

namespace HotUpdate.Base.Activity
{
    public interface IActivityDataManager : IDataManager
    {
        /// <summary>
        /// 活动数据集合
        /// </summary>
        IActivityDataCollection ActivityDataCollection { get; }
        
        bool TryGetData(int activityId, out ActivityData activityData);
    }
}
