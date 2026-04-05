using System;
using Newtonsoft.Json;

namespace HotUpdate.Core.Main.Settings
{
    /// <summary>
    /// 设置项
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class SettingItem<T> : ISettingItem
    {
        [JsonProperty] private ESettingType _settingType;
        [JsonProperty] private T _value;
        [JsonProperty] private bool _isRange;
        
        public SettingItem(ESettingType settingType, bool isRange)
        {
            _settingType = settingType;
            _isRange = isRange;
        }
        
        public object Value
        {
            get => _value;
            set => _value = (T)value;
        }

        public ESettingType SettingType
        {
            get => _settingType;
            set => _settingType = value;
        }

        public bool IsRange
        {
            get => _isRange;
            set => _isRange = value;
        }
    }
}
