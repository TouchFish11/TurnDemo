using Framework.Mock;
using Game.Battle;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;

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
            Register<IMainManager>(MainManager.Instance);
            Register<IMusicManager>(MusicManager.Instance);
            Register<IPoolManager>(PoolManager.Instance);
            Register<IResourcesManager>(ResourcesManager.Instance);
            Register<IScriptableObjectManager>(ScriptableObjectManager.Instance);
            Register<IServerManager>(ServerManager.Instance);
            Register<ITimerManager>(TimerManager.Instance);
            Register<IUIManager>(UIManager.Instance);
            Register<IVideoManager>(VideoManager.Instance);
            Register<IFactoryManager>(FactoryManager.Instance);
            Register<IVFXManager>(VFXManager.Instance);
            Register<IFloatingTextManager>(FloatingTextManager.Instance);
            
            // 非框架（主界面）
            Register<IDialogueManager>(DialogueManager.Instance);
            Register<ITaskManager>(TaskManager.Instance);
            Register<IPlayerManager>(PlayerManager.Instance);
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
            else
            {
                // 懒加载
                T instance = GetSingletonInstance<T>();
                Register(instance);
                return instance;
            }
        }

        /// <summary>
        /// 获取单例实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T GetSingletonInstance<T>() where T : class
        {
            Type interfaceType = typeof(T);

            // 框架相关
            if (interfaceType == typeof(ISceneManager))
            {
#if !UNITY_EDITOR || EDITOR_TEST_AB
                return SceneManager.Instance as T;
#else
                return MockSceneManager.Instance as T;
#endif
            }

            // 战斗相关
            if (interfaceType == typeof(IBattleManager))
            {
#if !UNITY_EDITOR || EDITOR_TEST_AB
                return BattleManager.Instance as T;
#else
                return BattleManager.Instance as T;
#endif
            }

            if (interfaceType == typeof(ITargetSelectManager))
            {
#if !UNITY_EDITOR || EDITOR_TEST_AB
                return TargetSelectManager.Instance as T;
#else
                return TargetSelectManager.Instance as T;
#endif
            }

            if (interfaceType == typeof(IDamageCalcManager))
            {
#if !UNITY_EDITOR || EDITOR_TEST_AB
                return DamageCalcManager.Instance as T;
#else
                return DamageCalcManager.Instance as T;
#endif
            }

            if (interfaceType == typeof(ISkillManager))
            {
#if !UNITY_EDITOR || EDITOR_TEST_AB
                return SkillManager.Instance as T;
#else
                return SkillManager.Instance as T;
#endif
            }

            LogManager.LogError($"未实现对应接口的单例实例获取逻辑");
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
