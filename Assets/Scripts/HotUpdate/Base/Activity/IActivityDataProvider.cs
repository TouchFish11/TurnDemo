using HotUpdate.Base.Provider;

namespace HotUpdate.Base.Activity
{
    public interface IActivityDataProvider : IDataProvider
    {
        /// <summary>
        /// 活动数据集合
        /// </summary>
        IActivityDataCollection ActivityDataCollection { get; }
    }
}
