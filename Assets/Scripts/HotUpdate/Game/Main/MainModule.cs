using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.HotUpdate;
using Core.Input.ActionAsset;
using Core.Mono;
using Core.Scene;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Module;
using HotUpdate.Base.Scene;
using HotUpdate.Common.Generated;
using HotUpdate.Game.Core;
using HotUpdate.Game.Main.FloatingText;
using HotUpdate.Game.Main.Player;
using HotUpdate.Game.Scene;
using HotUpdate.Game.VFX;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Main
{
    /// <summary>
    /// 游戏主场景模块
    /// </summary>
    public class MainModule : IMainModule
    {
        [Inject] private IInputSystem _inputSystem;
        [Inject] private IGameDataManager _gameDataManager;
        [Inject] private IFactoryManager _factoryManager;
        [Inject] private ISceneGenerator _sceneGenerator;
        [Inject] private ISceneManager _sceneManager;
        
        public void Register()
        {
            // 注册浮动文本管理器
            DIContainer.BindSingleton<IFloatingTextManager, FloatingTextManager>();
            // 注册玩家管理器
            DIContainer.BindSingleton<IPlayerManager, PlayerManager>();
            // 注册特效管理器
            DIContainer.BindSingleton<IVFXManager, VFXManager>();
            // 注册游戏管理器
            DIContainer.BindSingleton<IGameDataManager, GameDataManager>();
            // 注册场景生成器
            DIContainer.BindSingleton<ISceneGenerator, SceneGenerator>();
            // 注册工厂管理器
            DIContainer.BindSingleton<IFactoryManager, FactoryManager>();
        }

        public async Task InitModuleAsync()
        {
            // 初始化输入系统
            using var handle = await GameAsset.LoadAssetAsync<TextAsset>(FileUtility.InputActionLocalFileName);
            _inputSystem.InitInputSystem(handle.Asset.text);
            
            // 初始化热更工厂
            _factoryManager.BindFactory();
            
            // 初始化游戏数据
            // 先加载配置
            await _gameDataManager.LoadDataAsync();
            // 再加载数据，数据依赖配置
            await _gameDataManager.LoadDataAsync();
            
            // 预先生成所需组件类型
            PreGenerateRequireComponentTypes();

            await LoadSceneAsync();
            Logger.Log($"{nameof(MainModule)}: Initialization completed!!!");
        }
        
        /// <summary>
        /// 加载并切换场景
        /// </summary>
        /// <returns></returns>
        private async Task LoadSceneAsync()
        {
            await _sceneManager.LoadSceneAsync(AssetKeys.MainScene, LoadSceneMode.Single, null);
            // 初始化场景
            await _sceneGenerator.InitMainScene();
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