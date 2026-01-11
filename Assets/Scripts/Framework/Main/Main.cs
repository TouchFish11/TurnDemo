
namespace Framework
{
    /*
        示例脚本：如何启动框架，完成最基本的游戏初始化工作。
    */

    /// <summary>
    /// 游戏主函数入口
    /// </summary>
    public class Main : SingletonMono<Main>
    {
        private async void Start()
        {
            // 初始化服务定位器
            ServiceLocator.InitService();
            // 初始化主管理器
            await ServiceLocator.Get<IMainManager>().Init();
            // 尝试自动登录
            await ServiceLocator.Get<IServerManager>().TryAutoLogin();
        }
    }
}
