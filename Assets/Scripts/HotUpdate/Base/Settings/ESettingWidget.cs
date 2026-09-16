namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 设置控件类型，决定值的类型与存储方式：
    /// Slider = float（连续值，存 _floats），Dropdown = int（选项索引，存 _ints），Button = 无值（一次性动作，不存储）。
    /// </summary>
    public enum ESettingWidget
    {
        Slider,
        Dropdown,
        Button,
    }
}
