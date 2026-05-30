using System.Threading.Tasks;

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
    }
}
