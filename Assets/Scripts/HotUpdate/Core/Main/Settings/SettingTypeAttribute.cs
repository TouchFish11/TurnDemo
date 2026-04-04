using System;

namespace HotUpdate.Core.Main.Settings
{
    /// <summary>
    /// 设置类型特性
    /// </summary>
    public class SettingTypeAttribute : Attribute
    {
        /// <summary>
        /// 是否是范围类型，在设置界面中，设置UI的样式，true则使用滑动条的entry；否则使用下拉列表的entry
        /// </summary>
        public bool IsRange { get; set; }
    }
}
