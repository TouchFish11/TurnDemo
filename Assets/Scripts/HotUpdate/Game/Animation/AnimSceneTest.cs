using Core.AssetBundles.Update.Core;
using Core.DI;
using Core.EditorRes;
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
using Core.Time;
using Core.UI;
using Core.Video;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Object.Role.Warrior;
using UnityEngine;

namespace HotUpdate.Game.Animation
{
    public class AnimSceneTest : MonoBehaviour
    {
        public TextAsset textAsset;
        public Warrior Warrior;
        
        // Start is called before the first frame update
        void Start()
        {
            DIContainer.BindSingleton<IMonoAdapter, MonoAdapter>();
            DIContainer.BindSingleton<IMemoryMonitor, MemoryMonitor>();
            DIContainer.BindSingleton<IUWRManager, UWRManager>();
            DIContainer.BindSingleton<IPoolManager, PoolManager>();
            DIContainer.BindSingleton<IUIManager, UIManager>();
            DIContainer.BindSingleton<IAssetBundleUpdater, AssetBundleUpdater>();
            DIContainer.BindSingleton<IBinaryDataManager, BinaryDataManager>();
            DIContainer.BindSingleton<IEditorResManager, EditorResManager>();
            DIContainer.BindSingleton<IEventCenter, EventCenter>();
            //DIContainer.BindSingleton<IEventFactory, EventFactory>();
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

            var hotUpdateMockManager = DIContainer.Create<HotUpdateMockManager>();
            hotUpdateMockManager.LoadAssembliesAsync(null, null);
            
            EntityHelper.InitEntity(Warrior);
            // 挂载动画控制器组件
            var animatorComponent = Warrior.AddComponent<AnimatorComponent>();
            // animatorComponent._animatorComponentCore.InitTest(textAsset.text);
            // 挂载普通动画组件：处理玩家基础动画状态
            Warrior.AddComponent<NormalAnimationComponent>();

        }
    }
}
