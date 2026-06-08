using System;
using System.Collections.Generic;
using System.Reflection;
using Core.DI;
using Newtonsoft.Json;

namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 游戏设置
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class GameSettings
    {
        [JsonProperty] private Dictionary<ESettingType, ISettingItem> settings = new();
        
        public event Action<GameSettings> OnDataChanged;

        public float this[ESettingType type]
        {
            get => settings[type].Value;
            set
            {
                settings[type].Value = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public Dictionary<ESettingType, ISettingItem>.ValueCollection Values => settings.Values;
        public Dictionary<ESettingType, ISettingItem>.KeyCollection Keys => settings.Keys;

        public GameSettings()
        {
            foreach (var obj in Enum.GetValues(typeof(ESettingType)))
            {
                var type = (ESettingType)obj;
                var attribute = type.GetType().GetField(type.ToString()).GetCustomAttribute<SettingObjectAttribute>();
                if(attribute == null)
                    continue;

                var settingItem = DIContainer.Create<SettingItem>(parameterValues: new object[] { type, attribute.IsRange });
                settings.Add(type, settingItem);
            }
            
            // settings.Add(ESettingType.VolumeValue, new SettingItem(ESettingType.VolumeValue, true));
            // settings.Add(ESettingType.SFXValue, new SettingItem<float>(ESettingType.SFXValue, true));
            // settings.Add(ESettingType.VolumeOpen, new SettingItem<int>(ESettingType.VolumeOpen, false));
            // settings.Add(ESettingType.SFXOpen, new SettingItem<int>(ESettingType.SFXOpen, false));
            // settings.Add(ESettingType.TypeWriter, new SettingItem<int>(ESettingType.TypeWriter, false));
            // settings.Add(ESettingType.TargetFrameRateIndex, new SettingItem<int>(ESettingType.TargetFrameRateIndex, false));
        }
    }
}
