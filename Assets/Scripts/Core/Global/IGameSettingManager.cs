namespace Core.Global
{
    /// <summary>
    /// 游戏设置接口
    /// </summary>
    public interface IGameSettingManager
    {
        /// <summary>
        /// 游戏设置
        /// </summary>
        GameSettings GameSettings { get; }
    }
}
