using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 全局画质处理器。
    /// 选项索引即 QualitySettings 里的质量等级下标。
    /// 用 QualitySettings.names.Length 做边界保护：项目配置的质量等级不足时，越界的选项会被忽略。
    /// </summary>
    public class QualityLevelHandler : DropdownSettingHandler
    {
        public override void Execute(int optionIndex)
        {
            if (optionIndex >= 0 && optionIndex < QualitySettings.names.Length)
                QualitySettings.SetQualityLevel(optionIndex);
        }
    }
}
