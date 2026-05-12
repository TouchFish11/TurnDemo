using Core.Mono;
using HotUpdate.Base.Component;

namespace HotUpdate.Base
{
    /// <summary>
    /// 热更核心模块
    /// </summary>
    public class CoreModule
    {
        public int Priority => -1;

        public System.Threading.Tasks.Task InitModuleAsync()
        {
            PreGenerateRequireComponentTypes();
            // 注册游戏管理器
            //ServiceLocator.Register<IGameManager>(new GameManager(ServiceLocator.Get<IMonoAdapter>()));
            return System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// 预先生成所需组件类型
        /// </summary>
        private static void PreGenerateRequireComponentTypes()
        {
            var go = EngineUtility.Create("Prewarm");
            // 列举所有可能作为RequireComponent依赖的类型
            go.AddComponent<AnimatorComponent>();
            go.AddComponent<CharacterControllerComponent>();
            go.AddComponent<PlayerInputComponent>();
            // ...
            
            // 销毁
            EngineUtility.Destroy(go);
        }
    }
}
