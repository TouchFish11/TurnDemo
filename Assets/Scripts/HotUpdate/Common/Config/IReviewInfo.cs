namespace HotUpdate.Common.Config
{
    /// <summary>
    /// 回顾信息接口
    /// </summary>
    public interface IReviewInfo
    {
        public enum EReviewType : byte
        {
            Dialogue,
            Branch,
        }
        
        EReviewType ReviewType { get; }
        
        /// <summary>
        /// 获取回顾文本
        /// </summary>
        /// <returns></returns>
        string GetViewText();
    }
}
