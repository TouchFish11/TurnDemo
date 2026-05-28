using System;
using HotUpdate.Base.Enums;

namespace HotUpdate.Base.Data
{
    /// <summary>
    /// 确认数据
    /// </summary>
    public class ConfirmData
    {
        /// <summary>
        /// 确认界面标题
        /// </summary>
        public string ConfirmTitle { get; set; }
        
        /// <summary>
        /// 确认消息
        /// </summary>
        public string ConfirmMessage { get; set; }
        
        /// <summary>
        /// 内容数据
        /// </summary>
        public object ContentData { get; set; }
        
        /// <summary>
        /// 确认内容类型
        /// </summary>
        public EConfirmContent ConfirmContent { get; set; }
        
        /// <summary>
        /// 确认回调
        /// </summary>
        public Action OnConfirm { get; set; }
        
        /// <summary>
        /// 取消回调
        /// </summary>
        public Action OnCancel { get; set; }
    }
}