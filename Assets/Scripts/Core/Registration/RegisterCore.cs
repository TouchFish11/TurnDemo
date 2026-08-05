using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.AssetBundles.Update.Core;
using Core.DI;
using Core.EditorRes;
using Core.Global;
using Core.GlobalEvent;
using Core.HotUpdate;
using Core.Inputs;
using Core.Mono;
using Core.Music;
using Core.Net;
using Core.Pool;
using Core.PreLoad;
using Core.Res;
using Core.Scene;
using Core.Serialize.Binary;
using Core.Serialize.Json;
using Core.Systems.Memorys;
using Core.Tasks.Extensions;
using Core.Time;
using Core.UI;
using Core.Video;
using TaskFactory = Core.Tasks.TaskFactory;

namespace Core.Registration
{
    /// <summary>
    /// 注册框架
    /// </summary>
    public static class RegisterCore
    {
        /// <summary>
        /// 初始化框架
        /// </summary>
        public static async Task InitCore()
        {
            DIContainer.BindSingleton<IMonoAdapter, MonoAdapter>();
            DIContainer.BindSingleton<IMemoryMonitor, MemoryMonitor>();
            DIContainer.BindSingleton<IUWRManager, UWRManager>();
            DIContainer.BindSingleton<IPoolManager, PoolManager>();
            DIContainer.BindSingleton<IUIManager, UIManager>();
            DIContainer.BindSingleton<IAssetBundleManager, AssetBundleManager>();
            DIContainer.BindSingleton<IAssetBundleUpdater, AssetBundleUpdater>();
            DIContainer.BindSingleton<IBinaryDataManager, BinaryDataManager>();
            DIContainer.BindSingleton<IEditorResManager, EditorResManager>();
            DIContainer.BindSingleton<IEventCenter, EventCenter>();
            DIContainer.BindSingleton<IEventFactory, EventFactory>();
            DIContainer.BindSingleton<IInputSystem, InputSystem>();
            DIContainer.BindSingleton<IJsonManager, JsonManager>();
            DIContainer.BindSingleton<IMusicManager, MusicManager>();
            DIContainer.BindSingleton<IResourcesManager, ResourcesManager>();
            DIContainer.BindSingleton<ITimerManager, TimerManager>();
            DIContainer.BindSingleton<IVideoManager, VideoPlayManager>();
            DIContainer.BindSingleton<ISceneManager, SceneManager>();
            DIContainer.BindSingleton<IPreLoadManager, PreLoadManager>();
#if UNITY_EDITOR
            DIContainer.BindSingleton<IHotUpdateManager, HotUpdateMockManager>();
#else
            DIContainer.BindSingleton<IHotUpdateManager, HotUpdateManager>();
#endif
            // 配置实例
            TaskAwaiterExtensions.Configure(DIContainer.Resolve<TaskFactory>());
            
            // 初始化AB包管理器
            var assetBundleManager = DIContainer.Create<AssetBundleManager>(parameterValues: new object[]
            {
                GlobalSettings.Instance.criticalActiveThreshold,
                GlobalSettings.Instance.bundleSlidingWindowMaxCount,
                GlobalSettings.Instance.maxDurationPerWindow
            });
            await assetBundleManager.Init();
            // 初始化
            GameAsset.Init(assetBundleManager);
            // ...
        }
    }
}
