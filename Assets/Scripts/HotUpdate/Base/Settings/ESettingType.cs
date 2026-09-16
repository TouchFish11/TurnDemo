namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 设置类型，新增设置需在此加枚举值并在配置 SO 里加一条 SettingDefinition
    /// </summary>
    public enum ESettingType : byte
    {
        // ===== 已有 =====
        VolumeValue,
        SFXValue,
        VolumeOpen,
        SFXOpen,
        TypeWriter,
        TargetFrameRateIndex,

        // ===== 画面 =====
        FullscreenMode,
        Resolution,
        VSync,
        QualityLevel,
        ShadowQuality,
        ShadowDistance,
        AntiAliasing,
        TextureQuality,
        AnisotropicFiltering,
        FOV,
        Brightness,

        // ===== 音频 =====
        MasterVolume,
        VoiceVolume,
        AmbientVolume,
        UIVolume,
        AudioOutputMode,
        MuteInBackground,

        // ===== 玩法 =====
        BattleSpeed,
        AutoBattle,
        CameraSensitivity,

        // ===== 其他 =====
        Language,
        ShowFPS,
        Reset,
        ClearCache,
    }
}
