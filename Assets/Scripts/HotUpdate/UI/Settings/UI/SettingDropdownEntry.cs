using Core.DI;
using Core.UI;
using HotUpdate.Base.Settings;
using HotUpdate.UI.Settings.ViewModel;
using TMPro;

namespace HotUpdate.UI.Settings.UI
{
    /// <summary>
    /// 下拉列表设置
    /// </summary>
    public class SettingDropdownEntry : UIBehaviourBase, ISettingsEntry
    {
        [InjectUI] public TextMeshProUGUI txtName;
        [InjectUI] public TMP_Dropdown dpSettings;
        
        private SettingDropdownViewModel _settingDropdownViewModel;

        public void Init(string entryName, SettingDropdownViewModel settingDropdownViewModel)
        {
            txtName.text = entryName;
            dpSettings.AddOptions(settingDropdownViewModel.Options);
            
            // 数据导致UI更新
            settingDropdownViewModel.OptionIndex.Subscribe(optionIndex => dpSettings.SetValueWithoutNotify(optionIndex));
            _settingDropdownViewModel = settingDropdownViewModel;
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
            _settingDropdownViewModel.Dispose();
            _settingDropdownViewModel = null;
        }
    }
}
