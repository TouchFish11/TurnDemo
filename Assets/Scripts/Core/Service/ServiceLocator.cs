using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.AssetBundles.Update;
using Core.DataPersistence.Binary;
using Core.DataPersistence.Json;
using Core.EditorRes;
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
using Core.Systems.Memorys;
using Core.Time;
using Core.Video;

namespace Core.Service
{
    /// <summary>
    /// 服务定位器
    /// </summary>
    public class ServiceLocator
    {
        // 服务类型到实例映射
        private static readonly Dictionary<Type, object> TypeToServerMap = new Dictionary<Type, object>();

        private ServiceLocator()
        {

        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        public static void InitService()
        {
            // 继承Mono
            Register<IMonoAdapter>(MonoAdapter.Instance);
            Register<IQuitHandler>(QuitHandler.Instance);
            Register<IUWRManager>(UWRManager.Instance);
            Register<IMemoryMonitor>(MemoryMonitor.Instance);

            // 不继承Mono
            Register<IAssetBundleManager>(AssetBundleManager.Instance);
            Register<IAssetBundleUpdater>(AssetBundleUpdater.Instance);
            Register<IBinaryDataManager>(BinaryDataManager.Instance);
            Register<IEditorResManager>(EditorResManager.Instance);
            Register<IEventCenter>(EventCenter.Instance);
            Register<IInputSystem>(InputSystem.Instance);
            Register<IJsonManager>(JsonManager.Instance);
            Register<IMusicManager>(MusicManager.Instance);
            Register<IPoolManager>(PoolManager.Instance);
            Register<IResourcesManager>(ResourcesManager.Instance);
            Register<IScriptableObjectManager>(ScriptableObjectManager.Instance);
            Register<IServerManager>(ServerManager.Instance);
            Register<ITimerManager>(TimerManager.Instance);
            Register<IVideoManager>(VideoManager.Instance);
            Register<IFactoryManager>(FactoryManager.Instance);
            Register<IHotUpdateManager>(HotUpdateManager.Instance);
            Register<ISceneManager>(SceneManager.Instance);
            Register<IPreLoadManager>(PreLoadManager.Instance);

            // Test
#if !DISABLE_ADDRESSABLES
            Register<IAddressablesUpdater>(AddressablesUpdater.Instance);
            Register<IAddressablesManager>(AddressablesManager.Instance);
#endif
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
