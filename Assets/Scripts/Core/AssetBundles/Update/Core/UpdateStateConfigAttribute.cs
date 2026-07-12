using System;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 更新状态配置特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class UpdateStateConfigAttribute : Attribute
    {
        /// <summary>
        /// 执行顺序
        /// 越小越靠前
        /// </summary>
        public int Order { get; set; }       
        
        /// <summary>
        /// 是否启用该状态
        /// </summary>
        public bool IsEnabled { get; set; }  
    }
}
