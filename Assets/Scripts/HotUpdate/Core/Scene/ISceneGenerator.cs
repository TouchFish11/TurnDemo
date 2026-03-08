namespace HotUpdate.Core.Scene
{
    public interface ISceneGenerator
    {
        /// <summary>
        /// 初始化主游戏场景核心内容
        /// 异步创建NPC、玩家对象，初始化UI界面、飘字管理器等游戏元素
        /// </summary>
        System.Threading.Tasks.Task InitMainScene();

        /// <summary>
        /// 清理主游戏场景
        /// </summary>
        void ClearMainScene();
    }
}
