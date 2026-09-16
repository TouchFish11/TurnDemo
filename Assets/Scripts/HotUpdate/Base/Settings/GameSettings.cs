using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 游戏设置（运行时值），按控件类型分强类型字典存储
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class GameSettings
    {
        [JsonProperty] private Dictionary<ESettingType, float> _floats = new();
        [JsonProperty] private Dictionary<ESettingType, int> _ints = new();

        public event Action<GameSettings> OnDataChanged;

        public float GetFloat(ESettingType type) => _floats[type];
        public int GetInt(ESettingType type) => _ints[type];

        public void SetFloat(ESettingType type, float value)
        {
            _floats[type] = value;
            OnDataChanged?.Invoke(this);
        }

        public void SetInt(ESettingType type, int value)
        {
            _ints[type] = value;
            OnDataChanged?.Invoke(this);
        }

        /// <summary>
        /// 用定义初始化：字典缺失的键填默认值（兼容新增设置/旧存档）
        /// </summary>
        public void Initialize(GameSettingsConfig config)
        {
            foreach (var def in config.Settings)
            {
                switch (def.Widget)
                {
                    case ESettingWidget.Slider:
                        if (!_floats.ContainsKey(def.Type))
                        {
                            _floats[def.Type] = def.DefaultValue;
                        }
                        break;
                    case ESettingWidget.Dropdown:
                        if (!_ints.ContainsKey(def.Type))
                        {
                            _ints[def.Type] = def.DefaultIndex;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 重置所有设置为默认值
        /// </summary>
        public void Reset(GameSettingsConfig config)
        {
            _floats.Clear();
            _ints.Clear();
            Initialize(config);
        }
    }
}
