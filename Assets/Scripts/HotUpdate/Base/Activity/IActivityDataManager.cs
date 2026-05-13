namespace HotUpdate.Base.Activity
{
    public interface IActivityDataManager
    {
        /// <summary>
        /// 活动数据集合
        /// </summary>
        IActivityDataCollection ActivityDataCollection { get; }
    }
}
