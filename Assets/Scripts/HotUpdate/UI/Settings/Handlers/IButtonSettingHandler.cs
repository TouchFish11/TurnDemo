namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 按钮型设置处理器：对应 <see cref="ESettingWidget.Button"/>，一次性动作、不存值。
    /// 用于「重置所有设置」「清理缓存」这类点一下执行、没有状态的设置。
    /// </summary>
    public interface IButtonSettingHandler : ISettingHandler
    {
        /// <summary>
        /// 执行按钮动作
        /// </summary>
        void Execute();
    }
}
