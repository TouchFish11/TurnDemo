using Core.UI;
using HotUpdate.Core.Main.Settings.ViewModel;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.Main.Settings.UI
{
    /// <summary>
    /// 滑动条设置
    /// </summary>
    public class SettingSliderEntry : UIBehaviourBase, ISettingsEntry
    {
        [Inject] public TextMeshProUGUI txtName;
        [Inject] public TextMeshProUGUI txtVolume;
        [Inject] public Slider sliderRange;

        private SettingSliderViewModel _settingSliderViewModel;
        
        public void Init(string entryName, SettingSliderViewModel settingSliderViewModel)
        {
            txtName.text = entryName;
            settingSliderViewModel.ProgressSlider.Subscribe(volumeValue => sliderRange.value = volumeValue);
            settingSliderViewModel.ProgressText.Subscribe(volumeText => txtVolume.text = volumeText);
            _settingSliderViewModel = settingSliderViewModel;
            // 主动拉取数据
            settingSliderViewModel.RefleshUI();
        }

        protected override void OnSliderValueChanged(string sliderName, float value)
        {
            if (sliderName == nameof(sliderRange))
            {
                // 赋值滑动条的值
                _settingSliderViewModel.ProgressSlider.Value = value;
            }
        }

        protected override void OnDestroy()
        {
            _settingSliderViewModel.Dispose();
            _settingSliderViewModel = null;
        }
    }
}
