using System;

namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 设置对象特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SettingObjectAttribute : Attribute
    {
        public bool IsRange { get; private set; }
        
        public SettingObjectAttribute(bool isRange)
        {
            IsRange = isRange;
        }
    }
}
