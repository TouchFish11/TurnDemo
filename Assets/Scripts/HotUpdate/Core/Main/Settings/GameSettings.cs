using System;
using HotUpdate.Core.Data;

namespace HotUpdate.Core.Main.Settings
{
    /// <summary>
    /// 游戏设置
    /// </summary>
    [Serializable]
    public class GameSettings : IData<GameSettings>
    {
        [SettingType(IsRange = true)]
        private float volume;
        [SettingType(IsRange = true)]
        private float sound;
        [SettingType(IsRange = false)]
        private bool isOpenVolume;
        [SettingType(IsRange = false)]
        private bool isOpenSound;
        [SettingType(IsRange = false)]
        private bool enableTypewriter;
        [SettingType(IsRange = false)]
        private int targetFrameRateIndex;
        
        public event Action<GameSettings> OnDataChanged;

        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public float Sound
        {
            get => sound;
            set
            {
                sound = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public bool IsOpenSound
        {
            get => isOpenSound;
            set
            {
                isOpenSound = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public bool IsOpenVolume
        {
            get => isOpenVolume;
            set
            {
                isOpenVolume = value;
                OnDataChanged?.Invoke(this);
            }
        }
        
        public bool EnableTypewriter
        {
            get => enableTypewriter;
            set
            {
                enableTypewriter = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public int TargetFrameRateIndex
        {
            get => targetFrameRateIndex;
            set
            {
                targetFrameRateIndex = value;
                OnDataChanged?.Invoke(this);
            }
        }
    }
}
