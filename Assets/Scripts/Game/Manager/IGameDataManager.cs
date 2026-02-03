using System.Threading.Tasks;
using Core.Input.ActionAsset;
using Core.Input.CoreListen;
using Core.Music;
using Game.Tasks;

namespace Game.Manager
{
    /// <summary>
    /// 游戏数据管理器接口
    /// </summary>
    public interface IGameDataManager
    {
        /// <summary>
        /// 音乐数据
        /// </summary>
        MusicData MusicData { get; }
        
        /// <summary>
        /// 输入动作映射数据容器
        /// </summary>
        MainActionMapDataContainer InputActionContainer { get; }
        
        /// <summary>
        /// 输入数据容器
        /// </summary>
        InputDataContainer InputDataContainer { get; }

        /// <summary>
        /// 任务数据集合
        /// </summary>
        ITaskDataCollection TaskDataCollection { get; }

        Task InitData();
        
        Task SaveData();
    }
}
