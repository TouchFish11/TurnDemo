using Core.Inputs.ActionAsset;
using Core.Inputs.CoreListen;
using Core.Music;

namespace HotUpdate.Base.Collection
{
    public interface IMainDataCollection
    {
        /// <summary>
        /// 音乐数据
        /// </summary>
        MusicData MusicData { get; }

        /// <summary>
        /// 主动作行为映射数据容器
        /// </summary>
        MainActionMapDataContainer InputActionContainer { get; }

        /// <summary>
        /// 输入数据集合
        /// </summary>
        InputDataContainer InputDataContainer { get; }
    }
}
