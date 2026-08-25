using System;

namespace HotUpdate.Game.Activity.Core
{
    /// <summary>
    /// 活动ID特性
    /// </summary>
    public class ActivityIdAttribute : Attribute
    {
        /// <summary>
        /// 活动ID，与配置表对应
        /// </summary>
        public int ActivityId { get; set; }
        
        /// <summary>
        /// 指定该活动的处理器类型
        /// </summary>
        public Type ActivityContentHandler { get; set; }
    }
}
