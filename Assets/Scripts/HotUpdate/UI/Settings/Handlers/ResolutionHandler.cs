using System.Collections.Generic;
using HotUpdate.Base.Settings;
using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 分辨率处理器。
    /// 注意：Screen.resolutions 在 Editor 和部分平台返回不可靠（常是一堆低分辨率、缺 1920x1080），
    /// 所以这里改用「当前分辨率 + 常见分辨率白名单」，不再直接读 Screen.resolutions。
    /// </summary>
    public class ResolutionHandler : DropdownSettingHandler
    {
        // 常见分辨率白名单（宽, 高），当前分辨率会额外插到最前面
        private static readonly (int width, int height)[] CommonResolutions =
        {
            (3840, 2160), (2560, 1440), (1920, 1080), (1600, 900), (1280, 720),
            (2560, 1600), (1920, 1200), (1680, 1050), (1440, 900), (1366, 768), (1024, 768),
        };

        // 最终分辨率列表（当前分辨率 + 白名单去重），缓存起来供 GetOptions 和 Execute 共用同一份顺序
        private static readonly List<(int width, int height)> Resolutions = BuildResolutions();

        public override void Execute(int optionIndex)
        {
            if (optionIndex < 0 || optionIndex >= Resolutions.Count)
                return;

            var (w, h) = Resolutions[optionIndex];
            Screen.SetResolution(w, h, Screen.fullScreenMode);
        }

        /// <summary>
        /// 生成分辨率选项：Value 存 Resolutions 列表里的下标，Execute 按该下标取分辨率
        /// </summary>
        public static List<SettingOption> GetOptions()
        {
            var options = new List<SettingOption>(Resolutions.Count);
            for (var i = 0; i < Resolutions.Count; i++)
            {
                var (w, h) = Resolutions[i];
                options.Add(new SettingOption { Text = $"{w}x{h}", Value = i });
            }
            return options;
        }

        /// <summary>
        /// 构建分辨率列表：当前分辨率优先（保证至少一项真实有效），再拼白名单，按 (宽, 高) 去重
        /// </summary>
        private static List<(int width, int height)> BuildResolutions()
        {
            var seen = new HashSet<(int, int)>();
            var list = new List<(int, int)>();

            var cur = Screen.currentResolution;
            if (seen.Add((cur.width, cur.height)))
            {
                list.Add((cur.width, cur.height));
            }

            foreach (var r in CommonResolutions)
            {
                if (seen.Add(r))
                {
                    list.Add(r);
                }
            }

            return list;
        }
    }
}
