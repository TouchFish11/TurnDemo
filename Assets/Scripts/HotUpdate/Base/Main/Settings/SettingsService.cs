using UnityEngine;

namespace HotUpdate.Base.Main.Settings
{
    /// <summary>
    /// 设置服务
    /// </summary>
    public class SettingsService
    {
        /// <summary>
        /// 设置帧率
        /// </summary>
        /// <param name="frameRate"></param>
        public static void SetFrameRate(int frameRate)
        {
            if(frameRate == Application.targetFrameRate) return;
            Application.targetFrameRate = frameRate;
        }
    }
}
