using System;
using System.Collections.Generic;
using Core.UI;
using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.ViewModel
{
    /// <summary>
    /// 下拉列表设置ViewModel。
    /// 选项列表由外部传入：大多数来自 SettingDefinition.Options 的静态配置，
    /// 少数（如「分辨率」）由运行期动态生成，所以构造多收一个 options 参数而非直接读 definition.Options。
    /// </summary>
    public class SettingDropdownViewModel : IDisposable
    {
        /// <summary>
        /// 当前选中项的索引
        /// </summary>
        public ReactiveProperty<int> OptionIndex { get; protected set; }

        /// <summary>
        /// 选项显示文本列表（只存 Text 用于 UI 显示，语义值 Value 由 handler 使用）
        /// </summary>
        public List<string> Options { get; protected set; }

        protected SettingDropdownViewModel(GameSettings settings, SettingDefinition definition, List<SettingOption> options)
        {
            Options = new List<string>();
            if (options != null)
            {
                foreach (var opt in options)
                {
                    Options.Add(opt.Text);
                }
            }

            // 初始索引从 GameSettings 读取（用户上次保存的选择）
            OptionIndex = new ReactiveProperty<int>(settings.GetInt(definition.Type));
        }

        public void Dispose()
        {
            OptionIndex = null;
            Options = null;
        }
    }
}
