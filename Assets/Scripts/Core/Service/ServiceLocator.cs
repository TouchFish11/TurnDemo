using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.AssetBundles.Update.Core;
using Core.Collection;
using Core.EditorRes;
using Core.Extensions;
using Core.GlobalEvent;
using Core.HotUpdate;
using Core.Input.ActionAsset;
using Core.Log;
using Core.Mono;
using Core.Music;
using Core.Net;
using Core.Pool;
using Core.PreLoad;
using Core.Quit;
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
            // 继承Mono
            Register<IMonoAdapter>(MonoAdapter.Instance);
            Register<IQuitHandler>(QuitHandler.Instance);
            Register<IUWRManager>(UWRManager.Instance);
            Register<IMemoryMonitor>(MemoryMonitor.Instance);

            // 不继承Mono
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
            Register<IServerManager>(ServerManager.Instance);
            Register<ITimerManager>(TimerManager.Instance);
            Register<IVideoManager>(VideoManager.Instance);
            Register<IFactoryManager>(FactoryManager.Instance);
            Register<IHotUpdateManager>(HotUpdateManager.Instance);
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
            var initializables = TypeToServerMap.Values.ToArray(service => (IInitializable)service);
            var uniList = CollectionUtil.GetUniList<IInitializable>();
            uniList.AddRange(initializables);
            // 按优先级排序
            uniList.Sort((i1, i2) =>
            {
                if (i1.Priority > i2.Priority)
                {
                    return 1;
                }

                if (i1.Priority < i2.Priority)
                {
                    return -1;
                }

                return 0;
            });
            
            // 顺序初始化
            foreach (var initializable in uniList.List)
            {
                await initializable.InitAsync();
            }

            // 回收列表对象
            CollectionUtil.CollectUniList(uniList);
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
            LogManager.LogError($"注册类型，{type.Name}已存在");
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
            
            LogManager.LogError($"该类型不存在：{typeof(T)}");
            return null;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
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
