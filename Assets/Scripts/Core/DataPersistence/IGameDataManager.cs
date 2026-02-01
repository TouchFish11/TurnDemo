using System;
using System.Threading.Tasks;
using Core.InputSystem.ActionAsset;
using Core.InputSystem.CoreListen;
using Core.Music;

namespace Core.DataPersistence
{
    /// <summary>
    /// 游戏数据管理器接口
    /// </summary>
    public interface IGameDataManager
    {
        /// <summary>
        /// 初始化游戏数据回调
        /// </summary>
        event Func<Task> OnInitData;
        
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
        /// 异步初始化数据
        /// </summary>
        /// <returns></returns>
        Task InitDataAsync();

        /// <summary>
        /// 退出游戏数据保存事件
        /// </summary>
        event Func<Task> OnSaveData;
    }
}
