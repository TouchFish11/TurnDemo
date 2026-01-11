using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 全局服务定位器
    /// </summary>
    public class ServiceLocator
    {
        // 服务类型到服务的映射
        private static readonly Dictionary<Type, object> _typeToServerMap = new Dictionary<Type, object>();

        private ServiceLocator()
        {

        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        public static void InitService()
        {
            // 继承Mono
            Register<IMonoManager>(MonoManager.Instance);
            Register<IMouseManager>(MouseManager.Instance);
            Register<IQuitHandler>(QuitHandler.Instance);
            Register<IUWRManager>(UWRManager.Instance);

            // 不继承Mono
            Register<IAssetBundleManager>(AssetBundleManager.Instance);
            Register<IAssetBundleUpdater>(AssetBundleUpdater.Instance);
            Register<IBinaryDataManager>(BinaryDataManager.Instance);
            Register<IEditorResManager>(EditorResManager.Instance);
            Register<IEventCenter>(EventCenter.Instance);
            Register<IGameDataManager>(GameDataManager.Instance);
            Register<IInputSystem>(InputSystem.Instance);
            Register<IJsonManager>(JsonManager.Instance);
            Register<ILogManager>(LogManager.Instance);
            Register<IMainManager>(MainManager.Instance);
            Register<IMusicManager>(MusicManager.Instance);
            Register<IPoolManager>(PoolManager.Instance);
            Register<IResourcesManager>(ResourcesManager.Instance);
            Register<ISceneManager>(SceneManager.Instance);
            Register<IScriptableObjectManager>(ScriptableObjectManager.Instance);
            Register<IServerManager>(ServerManager.Instance);
            Register<ITimerManager>(TimerManager.Instance);
            Register<IUIManager>(UIManager.Instance);
            Register<IVideoManager>(VideoManager.Instance);
            Register<IFactoryManager>(FactoryManager.Instance);

            // 非框架（主界面）
            Register<IDialogueManager>(DialogueManager.Instance);
            Register<ITaskManager>(TaskManager.Instance);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_typeToServerMap.ContainsKey(type))
            {
                LogManager.LogError($"{type.Name}已存在，覆盖旧实例");
            }
            _typeToServerMap[type] = service;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_typeToServerMap.TryGetValue(type, out var service))
            {
                return service as T;
            }
            LogManager.LogError($"未找到{type.Name}");
            return null;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (_typeToServerMap.ContainsKey(type))
            {
                _typeToServerMap.Remove(type);
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public static void Clear()
        {
            _typeToServerMap.Clear();
        }
    }
}
