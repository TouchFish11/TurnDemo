using System;

namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 下拉选项
    /// </summary>
    [Serializable]
    public class SettingOption
    {
        public string Text;  // 显示文本
        public int Value;    // 选项语义值（帧率=30/60/120；开关=0/1）
    }
}
