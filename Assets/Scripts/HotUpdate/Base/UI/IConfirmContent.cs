using HotUpdate.Base.Data;

namespace HotUpdate.Base.UI
{
    /// <summary>
    /// 确认内容接口
    /// </summary>
    public interface IConfirmContent
    {
        /// <summary>
        /// 绘制提示内容
        /// </summary>
        /// <param name="confirmData">确认数据</param>
        void DrawContent(ConfirmData confirmData);
        
        /// <summary>
        /// 清除内容
        /// </summary>
        void ClearContent();
    }
}
