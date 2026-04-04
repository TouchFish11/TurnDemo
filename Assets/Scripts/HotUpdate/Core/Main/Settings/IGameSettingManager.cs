namespace HotUpdate.Core.Main.Settings
{
    /// <summary>
    /// 游戏设置接口
    /// </summary>
    public interface IGameSettingManager
    {
        /// <summary>
        /// 游戏设置数据
        /// </summary>
        GameSettings GameSettings { get; }
        

    }
}
