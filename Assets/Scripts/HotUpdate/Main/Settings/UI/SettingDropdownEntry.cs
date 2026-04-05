using Core.UI;
using HotUpdate.Core.Main.Settings.ViewModel;
using TMPro;

namespace HotUpdate.Main.Settings.UI
{
    /// <summary>
    /// 下拉列表设置
    /// </summary>
    public class SettingDropdownEntry : UIBehaviourBase, ISettingsEntry
    {
        [Inject] public TextMeshProUGUI txtName;
        [Inject] public TMP_Dropdown dpSettings;
        
        private SettingDropdownViewModel _settingDropdownViewModel;

        public void Init(string entryName, SettingDropdownViewModel settingDropdownViewModel)
        {
            txtName.text = entryName;
            dpSettings.AddOptions(settingDropdownViewModel.Options);
            
            // 数据导致UI更新
            settingDropdownViewModel.OptionIndex.Subscribe(optionIndex => dpSettings.value = optionIndex);
            _settingDropdownViewModel = settingDropdownViewModel;
            // 主动更新，触发事件
            settingDropdownViewModel.Update();
        }

        protected override void OnDropdownValueChanged(string dropdownName, int value)
        {
            if (dropdownName == nameof(dpSettings))
            {
                _settingDropdownViewModel.OptionIndex.Value = value;
            }
        }

        protected override void OnDestroy()
        {
            
        }
    }
}
