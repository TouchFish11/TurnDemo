using System;

namespace Framework
{
    /// <summary>
    /// 音乐数据
    /// </summary>
    [Serializable]
    public class MusicData
    {
        //音乐大小
        private float _musicValue = 1f;
        //音效大小
        private float _soundValue = 1f;
        //音乐是否开启
        private bool _musicIsOpen = true;
        //音效是否开启
        private bool _soundIsOpen = true;

        /// <summary>
        /// 音乐大小
        /// </summary>
        public float MusicValue { get => _musicValue; set => _musicValue = value; }

        /// <summary>
        /// 音效大小
        /// </summary>
        public float SoundValue { get => _soundValue; set => _soundValue = value; }

        /// <summary>
        /// 音乐是否开启
        /// </summary>
        public bool MusicIsOpen { get => _musicIsOpen; set => _musicIsOpen = value; }

        /// <summary>
        /// 音效是否开启
        /// </summary>
        public bool SoundIsOpen { get => _soundIsOpen; set => _soundIsOpen = value; }
    }
}
