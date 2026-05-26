using HotUpdate.Base.Collection;
using HotUpdate.Base.Settings;
using HotUpdate.Common.Config.Settings;

namespace HotUpdate.Base.Manager
{
    public interface IMainDataManager : IDataManager
    {
        /// <summary>
        /// 主数据集合
        /// </summary>
        IMainDataCollection MainDataCollection { get; }

        /// <summary>
        /// 游戏设置数据
        /// </summary>
        GameSettings GameSettings { get; }

        /// <summary>
        /// 游戏设置配置
        /// </summary>
        GameSettingsConfig GameSettingsConfig { get; }
    }
}
