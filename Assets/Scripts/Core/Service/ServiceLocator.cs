using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.AssetBundles.Update.Core;
using Core.Collection;
using Core.EditorRes;
using Core.Extensions;
using Core.Global;
using Core.GlobalEvent;
using Core.HotUpdate;
using Core.Input.ActionAsset;
using Core.Log;
using Core.Mono;
using Core.Mono.MonoFunction;
using Core.Music;
using Core.Net;
using Core.Pool;
using Core.PreLoad;
using Core.Reflection;
using Core.Res;
using Core.Scene;
using Core.ScriptableObject;
using Core.Serialize.Binary;
using Core.Serialize.Json;
using Core.Singleton;
using Core.Systems.Memorys;
using Core.Time;
using Core.UI;
using Core.Video;

namespace Core.Service
{
    /// <summary>
    /// 服务定位器
    /// </summary>
    public class ServiceLocator
    {
        // 服务类型到实例映射
        private static readonly Dictionary<Type, object> TypeToServerMap = new();

        private ServiceLocator()
        {

        }

        /// <summary>
        /// 异步注册服务
        /// </summary>
        public static Task RegisterServices()
        {
            // Mono适配器
            Register<IMonoAdapter>(MonoAdapter.Instance);
            
            Register<ILogManager>(LogManager.Instance);
            Register<IMemoryMonitor>(MemoryMonitor.Instance);
            Register<IUWRManager>(UWRManager.Instance);
            Register<IPoolManager>(PoolManager.Instance);
            Register<IUIManager>(UIManager.Instance);
            Register<IAssetBundleManager>(AssetBundleManager.Instance);
            Register<IAssetBundleUpdater>(AssetBundleUpdater.Instance);
            Register<IBinaryDataManager>(BinaryDataManager.Instance);
            Register<IEditorResManager>(EditorResManager.Instance);
            Register<IEventCenter>(EventCenter.Instance);
            Register<IInputSystem>(InputSystem.Instance);
            Register<IJsonManager>(JsonManager.Instance);
            Register<IMusicManager>(MusicManager.Instance);
            Register<IResourcesManager>(ResourcesManager.Instance);
            Register<IScriptableObjectManager>(ScriptableObjectManager.Instance);
            Register<ITimerManager>(TimerManager.Instance);
            Register<IVideoManager>(VideoManager.Instance);
            Register<IFactoryManager>(FactoryManager.Instance); 
#if UNITY_EDITOR
            Register<IHotUpdateManager>(HotUpdateMockManager.Instance);
#else
            Register<IHotUpdateManager>(HotUpdateManager.Instance);
#endif
            Register<ISceneManager>(SceneManager.Instance);
            Register<IPreLoadManager>(PreLoadManager.Instance);
            
            // 初始化服务
            return InitServices();
            
            // Test
#if !DISABLE_ADDRESSABLES
            Register<IAddressablesUpdater>(AddressablesUpdater.Instance);
            Register<IAddressablesManager>(AddressablesManager.Instance);
#endif
        }

        /// <summary>
        /// 异步初始化所有服务
        /// 按优先级初始化
        /// </summary>
        private static async Task InitServices()
        {
            await InitInitializable();
            InitQuit();
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        private static async Task InitInitializable()
        {
            var initializables = new List<IInitializable>(TypeToServerMap.Values.ToArray(service =>
            {
                if (service is IInitializable initializable) return initializable;
                return null;
            }));
            
            // 按优先级排序
            initializables.Sort((i1, i2) =>
            {
                if (i1.InitPriority > i2.InitPriority) return 1;
                if (i1.InitPriority < i2.InitPriority) return -1;
                return 0;
            });
            
            // 顺序初始化
            foreach (var initializable in initializables)
            {
                await initializable.InitAsync();
            }
        }

        /// <summary>
        /// 注册到适配器中
        /// </summary>
        private static void InitQuit()
        {
            var applicationExitNotifies = TypeToServerMap.Values.ToArray(service =>
            {
                if (service is IApplicationExitNotify applicationExitNotify) return applicationExitNotify;
                return null;
            });
            Get<IMonoAdapter>().AddApplicationExitNotifies(applicationExitNotifies);
        }
        
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (TypeToServerMap.TryAdd(type, service))
            {
                return;
            }
            LogManager.LogError($"{nameof(ServiceLocator)}.{nameof(Register)}：注册类型{type.Name}已存在");
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (TypeToServerMap.TryGetValue(type, out var service))
            {
                return service as T;
            }
            
            LogManager.LogError($"{nameof(ServiceLocator)}.{nameof(Get)}：该类型{typeof(T)}不存在");
            return null;
        }

        /// <summary>
        /// 注销类型实例
        /// 若类型实现IDestroyable接口，会调用其OnDestroy方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (!TypeToServerMap.TryGetValue(type, out var service))
            {
                return;
            }

            if (service is IDestroyable destroyable)
            {
                destroyable.OnDestroy();
            }
            
            TypeToServerMap.Remove(type);
        }
        
        /// <summary>
        /// 清理
        /// </summary>
        public static void Clear()
        {
            TypeToServerMap.Clear();
        }
    }
}
