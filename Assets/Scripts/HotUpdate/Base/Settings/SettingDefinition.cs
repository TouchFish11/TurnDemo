using System;
using System.Collections.Generic;

namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 设置项定义，一个设置的完整描述
    /// </summary>
    [Serializable]
    public class SettingDefinition
    {
        public ESettingType Type;
        public string Name;                 // 显示名
        public ESettingWidget Widget;       // 控件类型
        public ESettingCategory Category;   // 分类（页签）
        public float DefaultValue;          // Slider 默认值
        public int DefaultIndex;            // Dropdown 默认索引
        public float Min;                   // Slider 最小值
        public float Max;                   // Slider 最大值
        public float DisplayMultiplier = 10f; // Slider 显示缩放（存 0-1、显示 0-10 则填 10）
        public List<SettingOption> Options; // Dropdown 选项
    }
}
