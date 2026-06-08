namespace HotUpdate.Base.Settings
{
    /// <summary>
    /// 设置类型，新增设置只需新增枚举
    /// </summary>
    public enum ESettingType : byte
    {
        /// <summary>
        /// 音量值
        /// </summary>
        [SettingObject(true)]
        VolumeValue,
        
        /// <summary>
        /// 音效值
        /// </summary>
        [SettingObject(true)]
        SFXValue,
        
        /// <summary>
        /// 音量开关
        /// </summary>
        [SettingObject(false)]
        VolumeOpen,
        
        /// <summary>
        /// 音效开关
        /// </summary>
        [SettingObject(false)]
        SFXOpen,
        
        /// <summary>
        /// 对话打字机效果
        /// </summary>
        [SettingObject(false)]
        TypeWriter,
        
        /// <summary>
        /// 帧率索引
        /// </summary>
        [SettingObject(false)]
        TargetFrameRateIndex,
    }
}
