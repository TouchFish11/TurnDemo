using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.Scene
{
    /// <summary>
    /// 场景管理器接口
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        /// 异步加载场景的方法
        /// </summary>
        /// <param name="scenePath">要加载的场景路径</param>
        /// <param name="mode">场景加载模式（叠加/替换）</param>
        /// <param name="onLoadProgress">加载进度回调，参数为0~1的进度值</param>
        Task LoadSceneAsync(string scenePath, LoadSceneMode mode, Action<float> onLoadProgress);
    }
}
