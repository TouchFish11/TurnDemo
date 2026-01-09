using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 主管理器
    /// </summary>
    public class MainManager : SingletonBase<MainManager>, IMainManager
    {
        private MainManager()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public async Task Init()
        {
            // 初始化工厂
            ServiceLocator.Instance.Get<IFactoryManager>().InitFactorys();
            // 激活处理器
            ServiceLocator.Instance.Get<IQuitHandler>().ActiveHandler();
            // 初始化AB包资源
            await ServiceLocator.Instance.Get<AssetBundleManager>().Init();
            // 初始化UI管理器
            await ServiceLocator.Instance.Get<IUIManager>().InitUIManagerAsync();
            // 初始化游戏数据
            await ServiceLocator.Instance.Get<IGameDataManager>().InitDataAsync();
            // 初始化Lua管理器
            // await EnvManager.Instance.InitLuaAsync("Main");
            // 初始化更新器
            ServiceLocator.Instance.Get<IAssetBundleUpdater>().Init();
        }
    }
}
