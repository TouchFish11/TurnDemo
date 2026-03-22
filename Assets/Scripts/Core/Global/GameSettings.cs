using System;

namespace Core.Global
{
    /// <summary>
    /// 游戏设置
    /// </summary>
    [Serializable]
    public class GameSettings
    {
        [SettingType(IsRange = true)]
        public float volume;
        [SettingType(IsRange = true)]
        public float sound;
        [SettingType(IsRange = false)]
        public bool isOpenVolume;
        [SettingType(IsRange = false)]
        public bool isOpenSound;
        [SettingType(IsRange = false)]
        public bool enableTypewriter;
        [SettingType(IsRange = false)]
        public int targetFrameRate;
    }
}
