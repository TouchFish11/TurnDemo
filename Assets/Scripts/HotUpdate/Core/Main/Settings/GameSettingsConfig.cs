using System;
using System.Collections.Generic;

namespace HotUpdate.Core.Main.Settings
{
    /// <summary>
    /// 游戏设置配置，定义可供选择的设置类型和内容
    /// </summary>
    [Serializable]
    public class GameSettingsConfig
    {
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
