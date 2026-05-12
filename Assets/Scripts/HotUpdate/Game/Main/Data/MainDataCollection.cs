using Core.Input.ActionAsset;
using Core.Input.CoreListen;
using Core.Music;
using HotUpdate.Base.Main;

namespace HotUpdate.Game.Main.Data
{
    /// <summary>
    /// 主数据集合
    /// </summary>
    public class MainDataCollection : IMainDataCollection
    {
        /// <summary>
        /// 音乐数据
        /// </summary>
        public MusicData MusicData { get; set; }

        /// <summary>
        /// 主动作行为映射数据容器
        /// </summary>
        public MainActionMapDataContainer InputActionContainer { get; set;}

        /// <summary>
        /// 输入数据集合
        /// </summary>
        public InputDataContainer InputDataContainer { get; set; }
    }
}
