using System.Collections.Generic;
using Core.DI;
using HotUpdate.Base.Settings;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 设置处理器注册表，加设置项的自定义应用逻辑时在此注册 handler
    /// </summary>
    public class SettingHandlerRegistry
    {
        private readonly Dictionary<ESettingType, ISettingHandler> _handlers = new()
        {
            // ===== 画面（直接生效） =====
            { ESettingType.FullscreenMode, DIContainer.Create<FullscreenModeHandler>() },
            { ESettingType.Resolution, DIContainer.Create<ResolutionHandler>() },
            { ESettingType.VSync, DIContainer.Create<VSyncHandler>() },
            { ESettingType.TargetFrameRateIndex, DIContainer.Create<FrameRateSettingHandler>() },
            { ESettingType.QualityLevel, DIContainer.Create<QualityLevelHandler>() },
            { ESettingType.ShadowQuality, DIContainer.Create<ShadowQualityHandler>() },
            { ESettingType.ShadowDistance, DIContainer.Create<ShadowDistanceHandler>() },
            { ESettingType.AntiAliasing, DIContainer.Create<AntiAliasingHandler>() },
            { ESettingType.TextureQuality, DIContainer.Create<TextureQualityHandler>() },
            { ESettingType.AnisotropicFiltering, DIContainer.Create<AnisotropicFilteringHandler>() },

            // ===== 画面（占位，需相机/后处理） =====
            { ESettingType.FOV, DIContainer.Create<SliderSettingHandler>() },
            { ESettingType.Brightness, DIContainer.Create<SliderSettingHandler>() },

            // ===== 音频（占位，需 AudioMixer） =====
            { ESettingType.VolumeValue, DIContainer.Create<SliderSettingHandler>() },
            { ESettingType.SFXValue, DIContainer.Create<SliderSettingHandler>() },
            { ESettingType.MasterVolume, DIContainer.Create<SliderSettingHandler>() },
            { ESettingType.VoiceVolume, DIContainer.Create<SliderSettingHandler>() },
            { ESettingType.AmbientVolume, DIContainer.Create<SliderSettingHandler>() },
            { ESettingType.UIVolume, DIContainer.Create<SliderSettingHandler>() },
            { ESettingType.VolumeOpen, DIContainer.Create<DropdownSettingHandler>() },
            { ESettingType.SFXOpen, DIContainer.Create<DropdownSettingHandler>() },
            { ESettingType.AudioOutputMode, DIContainer.Create<DropdownSettingHandler>() },
            { ESettingType.MuteInBackground, DIContainer.Create<DropdownSettingHandler>() },

            // ===== 玩法 =====
            { ESettingType.TypeWriter, DIContainer.Create<DropdownSettingHandler>() }, // 已接 DialogueManager
            { ESettingType.BattleSpeed, DIContainer.Create<DropdownSettingHandler>() },
            { ESettingType.AutoBattle, DIContainer.Create<DropdownSettingHandler>() },
            { ESettingType.CameraSensitivity, DIContainer.Create<SliderSettingHandler>() },

            // ===== 其他 =====
            { ESettingType.Language, DIContainer.Create<DropdownSettingHandler>() },
            { ESettingType.ShowFPS, DIContainer.Create<DropdownSettingHandler>() },
            { ESettingType.Reset, DIContainer.Create<ResetSettingsHandler>() },
            { ESettingType.ClearCache, DIContainer.Create<ClearCacheHandler>() },
        };

        public ISettingHandler Get(ESettingType type) => _handlers[type];

        /// <summary>
        /// 应用所有已保存的设置（启动时调用），让全屏/画质/垂直同步/帧率等设置立即生效，
        /// 而不是等到打开设置界面改一次才生效。
        /// 按钮型（Reset/ClearCache）是一次性动作、无初始值，跳过。
        /// </summary>
        public void ApplyAll(GameSettings settings, GameSettingsConfig config)
        {
            foreach (var def in config.Settings)
            {
                if (def.Widget == ESettingWidget.Button)
                    continue;

                if (!_handlers.TryGetValue(def.Type, out var handler))
                    continue;

                switch (def.Widget)
                {
                    case ESettingWidget.Slider:
                        ((ISliderSettingHandler)handler).Excute(settings.GetFloat(def.Type));
                        break;
                    case ESettingWidget.Dropdown:
                        ((IDropdownSettingHandler)handler).Execute(settings.GetInt(def.Type));
                        break;
                }
            }
        }
    }
}
