namespace HotUpdate.Base.UI
{
    /// <summary>
    /// 阻挡操作接口，界面UI是否阻挡玩家操作
    /// </summary>
    public interface IBlockOperation
    {
        /// <summary>
        /// 是否阻挡玩家操作
        /// </summary>
        bool BlockOperation { get; }
        
        /// <summary>
        /// 设置阻挡标识，修改BlockOperation属性
        /// </summary>
        /// <param name="isBlock"></param>
        void SetBlock(bool isBlock);
    }
}
