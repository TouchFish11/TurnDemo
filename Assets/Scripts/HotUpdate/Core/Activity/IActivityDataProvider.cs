using HotUpdate.Core.Provider;

namespace HotUpdate.Core.Activity
{
    public interface IActivityDataProvider : IDataProvider
    {
        /// <summary>
        /// 活动数据集合
        /// </summary>
        IActivityDataCollection ActivityDataCollection { get; }
    }
}
