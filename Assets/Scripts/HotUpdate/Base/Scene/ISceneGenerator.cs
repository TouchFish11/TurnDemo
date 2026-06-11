using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace HotUpdate.Base.Scene
{
    public interface ISceneGenerator
    {
        /// <summary>
        /// 初始化主游戏场景核心内容
        /// 异步创建NPC、玩家对象，初始化UI界面、飘字管理器等游戏元素
        /// </summary>
        /// <param name="sceneId">场景ID，读取配置表加载指定场景</param>
        Task InitMainScene(int sceneId);

        /// <summary>
        /// 清理主游戏场景
        /// </summary>
        void ClearMainScene();

        /// <summary>
        /// 初始化场景，对底层场景加载的封装
        /// </summary>
        /// <param name="sceneId">场景名称</param>
        /// <param name="mode">加载模式</param>
        /// <param name="onLoadProgress">加载进度回调，可为null</param>
        /// <param name="sceneConfig">场景配置（当前仅占位，忽略）</param>
        Task InitSceneAsync(string sceneId, LoadSceneMode mode, Action<float> onLoadProgress, object sceneConfig = null);
    }
}
