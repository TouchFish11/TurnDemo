using System;
using System.Collections.Generic;

namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 游戏设置配置，定义所有可配置的设置项
    /// </summary>
    [Serializable]
    public class GameSettingsConfig
    {
        public List<SettingDefinition> Settings = new();
    }
}
