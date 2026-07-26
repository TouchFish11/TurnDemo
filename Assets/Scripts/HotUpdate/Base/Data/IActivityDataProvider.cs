using HotUpdate.Base.Collection;

namespace HotUpdate.Base.Data
{
    public interface IActivityDataProvider : IDataProvider
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
