using Core.UI;
using HotUpdate.UI.Settings.ViewModel;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Settings.UI
{
    /// <summary>
    /// 滑动条设置项UI条目
    /// </summary>
    public class SettingSliderEntry : UIBehaviourBase, ISettingsEntry
    {
        [InjectUI] public TextMeshProUGUI txtName;
        [InjectUI] public TextMeshProUGUI txtVolume;
        [InjectUI] public Slider sliderRange;

        private SettingSliderViewModel _settingSliderViewModel;
        
        public void Init(string entryName, SettingSliderViewModel settingSliderViewModel)
        {
            txtName.text = entryName;
            // 订阅响应事件
            settingSliderViewModel.Progress.Subscribe(value =>
            {
                sliderRange.SetValueWithoutNotify(value);
                txtVolume.text = $"{value}";
            });
            
            _settingSliderViewModel = settingSliderViewModel;
        }

        protected override void OnSliderValueChanged(string sliderName, float value)
        {
            if (sliderName == nameof(sliderRange))
            {
                // 赋值滑动条的值
                _settingSliderViewModel.Progress.Value = value;
            }
        }

        protected override void OnDestroy()
        {
            _settingSliderViewModel.Dispose();
            _settingSliderViewModel = null;
        }
    }
}
