using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 主管理器
    /// </summary>
    public class MainManager : SingletonBase<MainManager>
    {
        private MainManager() { }

        /// <summary>
        /// 初始化
        /// </summary>
        public async Task Init()
        {
            // 激活处理器
            QuitHandler.Instance.ActiveHandler();
            // 初始化AB包资源
            await AssetBundleManager.Instance.Init();
            // 初始化UI管理器
            await UIManager.Instance.InitUIManagerAsync();
            // 初始化游戏数据
            await GameDataMgr.Instance.InitDataAsync();
            // 初始化Lua管理器
            // await EnvManager.Instance.InitLuaAsync("Main");
            // 初始化更新器
            AssetBundleUpdater.Instance.Init();
        }
    }
}
