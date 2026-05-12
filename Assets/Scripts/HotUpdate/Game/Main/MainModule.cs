using System.Threading.Tasks;
using Core.Log;

namespace HotUpdate.Game.Main
{
    /// <summary>
    /// 热更主界面模块
    /// </summary>
    public class MainModule
    {
        public int Priority => 0;
        
        public Task InitModuleAsync()
        {
            // // 注册浮动文本管理器
            // DIContainer.GetInstance.Register<IFloatingTextManager>(new FloatingTextManager(
            //     DIContainer.GetInstance<IPrefabLoader>(), 
            //     DIContainer.GetInstance<IMonoAdapter>()));
            // // 注册玩家管理器
            // DIContainer.GetInstance.Register<IPlayerManager>(new PlayerManager(
            //     DIContainer.GetInstance<IPrefabLoader>(), 
            //     DIContainer.GetInstance<IEventCenter>()));
            // // 注册特效管理器
            // DIContainer.GetInstance.Register<IVFXManager>(new VFXManager(
            //     DIContainer.GetInstance<IMonoAdapter>(), 
            //     DIContainer.GetInstance<IPrefabLoader>()));
            // // 注册主数据提供器
            // DIContainer.GetInstance<IGameManager>().GameDataManager.RegisterProvider(
            //     typeof(IMainDataProvider), 
            //     new MainDataProvider(DIContainer.GetInstance<IBinaryDataManager>(), DIContainer.GetInstance<IJsonManager>()));
            // // 注册主模块UIhelper
            // DIContainer.GetInstance.Register<IMainUiHelper>(new MainUiHelper(DIContainer.GetInstance<IUIManager>()));
            // // 初始化场景生成器
            // SceneGeneratorHelper.Init(new SceneGenerator());
            Logger.Log($"{nameof(MainModule)}.{nameof(InitModuleAsync)}:Main module initialization completed");
            
            return Task.CompletedTask;
        }
    }
}
