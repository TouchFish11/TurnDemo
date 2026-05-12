namespace HotUpdate.Base.Main.Settings
{
    /// <summary>
    /// 设置项接口
    /// </summary>
    public interface ISettingItem
    {
        /// <summary>
        /// 设置的值
        /// </summary>
        object Value { get; set; }
        
        /// <summary>
        /// 设置项类型
        /// </summary>
        ESettingType SettingType { get; }
        
        /// <summary>
        /// 设置项的值的取值范围，若为true，则是范围类型，即使用滑动条类型的UI显示该值；若为false，则是离散值，可使用下拉菜单的UI显示该值
        /// </summary>
        bool IsRange { get; }
    }
}
