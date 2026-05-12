using HotUpdate.Base.Main.Settings;
using HotUpdate.Base.Provider;

namespace HotUpdate.Base.Main
{
    public interface IMainDataProvider : IDataProvider
    {
        /// <summary>
        /// 游戏设置数据
        /// </summary>
        GameSettings GameSettings { get; }
        
        /// <summary>
        /// 游戏设置配置
        /// </summary>
        GameSettingsConfig GameSettingsConfig { get; }

        /// <summary>
        /// 主数据集合
        /// </summary>
        IMainDataCollection MainDataCollection { get; }
    }
}
