using Core.UI;
using HotUpdate.Base.Settings;
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
        private float _displayMultiplier;

        public void Init(SettingDefinition definition, SettingSliderViewModel settingSliderViewModel)
        {
            txtName.text = definition.Name;
            _displayMultiplier = definition.DisplayMultiplier <= 0f ? 1f : definition.DisplayMultiplier;
            sliderRange.minValue = definition.Min * _displayMultiplier;
            sliderRange.maxValue = definition.Max * _displayMultiplier;
            
            // 订阅响应事件
            settingSliderViewModel.Progress.Subscribe(value =>
            {
                sliderRange.SetValueWithoutNotify(value * _displayMultiplier);
                txtVolume.text = ((int)(value * _displayMultiplier)).ToString();
            });
            _settingSliderViewModel = settingSliderViewModel;
        }

        protected override void OnSliderValueChanged(string sliderName, float value)
        {
            if (sliderName == nameof(sliderRange))
            {
                if(_settingSliderViewModel == null)
                    return;
                
                // 显示值换算回原始值存储
                _settingSliderViewModel.Progress.Value = value / _displayMultiplier;
            }
        }

        protected override void OnDisable()
        {
            _settingSliderViewModel.Dispose();
            _settingSliderViewModel = null;
        }
    }
}
