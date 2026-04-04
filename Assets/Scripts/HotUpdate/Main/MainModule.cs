using System.Threading.Tasks;
using Core.Global;
using Core.GlobalEvent;
using Core.Loader.Object;
using Core.Log;
using Core.Mono;
using Core.Serialize.Binary;
using Core.Serialize.Json;
using Core.Service;
using Core.UI;
using HotUpdate.Core.Main;
using HotUpdate.Core.Main.Settings;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;
using HotUpdate.Core.Scene;
using HotUpdate.Core.UI.Helper;
using HotUpdate.Core.VFX;
using HotUpdate.Main.Data;
using HotUpdate.Main.FloatingText;
using HotUpdate.Main.Player;
using HotUpdate.Main.Settings;
using HotUpdate.Main.VFX;

namespace HotUpdate.Main
{
    /// <summary>
    /// 热更主界面模块
    /// </summary>
    public class MainModule : IMainModule
    {
        public int Priority => 0;
        
        public Task InitModuleAsync()
        {
            // 注册浮动文本管理器
            ServiceLocator.Register<IFloatingTextManager>(new FloatingTextManager(
                ServiceLocator.Get<IPrefabLoader>(), 
                ServiceLocator.Get<IMonoAdapter>()));
            // 注册玩家管理器
            ServiceLocator.Register<IPlayerManager>(new PlayerManager(
                ServiceLocator.Get<IPrefabLoader>(), 
                ServiceLocator.Get<IEventCenter>()));
            // 注册特效管理器
            ServiceLocator.Register<IVFXManager>(new VFXManager(
                ServiceLocator.Get<IMonoAdapter>(), 
                ServiceLocator.Get<IPrefabLoader>()));
            // 注册主数据提供器
            ServiceLocator.Get<IGameManager>().GameDataManager.RegisterProvider(
                typeof(IMainDataProvider), 
                new MainDataProvider(ServiceLocator.Get<IBinaryDataManager>(), ServiceLocator.Get<IJsonManager>()));
            // 注册主模块UIhelper
            ServiceLocator.Register<IMainUiHelper>(new MainUiHelper(ServiceLocator.Get<IUIManager>()));
            // 初始化场景生成器
            SceneGeneratorHelper.Init(new SceneGenerator());
            LogManager.Log($"{nameof(MainModule)}.{nameof(InitModuleAsync)}:Main module initialization completed");
            
            return Task.CompletedTask;
        }
    }
}
