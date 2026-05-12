using System;
using System.Collections.Generic;

namespace HotUpdate.Base.Main.Settings
{
    /// <summary>
    /// 游戏设置配置，定义可供选择的设置类型和内容
    /// </summary>
    [Serializable]
    public class GameSettingsConfig
    {
        // UI名称
        public string volumeItemName;
        public string sfxItemName;
        public string volumeOpenItemName;
        public string sfxOpenItemName;
        public string typeWriterItemName;
        public string frameRateItemName;
        
        // 音乐选项
        public List<string> volumeOpts;
        // 音效选项
        public List<string> sfxOpts;
        // 打字机效果选项
        public List<string> typeWriterOpts;
        // 帧率选项
        public List<int> framerates;
    }
}
