using Core.UI;
using HotUpdate.Core.Main.Settings.ViewModel;
using TMPro;

namespace HotUpdate.Main.Settings.UI
{
    /// <summary>
    /// 下拉列表设置
    /// </summary>
    public class SettingDrowdownEntry : UIBehaviourBase, ISettingsEntry
    {
        [Inject] public TextMeshProUGUI txtName;
        [Inject] public TMP_Dropdown dpSettings;
        private SettingDrowdownViewModel _settingDrowdownViewModel;

        public void Init(string entryName, SettingDrowdownViewModel settingDrowdownViewModel)
        {
            txtName.text = entryName;
            // 数据导致UI更新
            _settingDrowdownViewModel.OptionIndex.OnValueChanged += optionIndex => dpSettings.value = optionIndex;
            _settingDrowdownViewModel.Options.OnValueChanged += options => dpSettings.AddOptions(options);
            // 主动更新，触发事件
            settingDrowdownViewModel.Update();
            _settingDrowdownViewModel = settingDrowdownViewModel;
        }

        protected override void OnDropdownValueChanged(string dropdownName, int value)
        {
            if (dropdownName == nameof(dpSettings))
            {
                _settingDrowdownViewModel.OptionIndex.Value = value;
            }
        }
    }
}
