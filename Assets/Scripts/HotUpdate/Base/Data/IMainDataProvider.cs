using System.Threading.Tasks;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Settings;

namespace HotUpdate.Base.Data
{
    public interface IMainDataProvider : IDataProvider
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

        /// <summary>
        /// 只加载游戏设置（轻量，启动早期调用），与 LoadDataAsync 的重配置加载分离
        /// </summary>
        Task LoadSettingsAsync();
    }
}
