using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Inputs.ActionAsset;
using Core.Mono;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Scene;
using HotUpdate.Game.Animation.Component;
using UnityEngine;

namespace HotUpdate.Game.Main
{
    /// <summary>
    /// 玩家初始化器，统一初始化玩家相关内容
    /// </summary>
    public class PlayerInitializer
    {
        [Inject] private IInputSystem _inputSystem;
        [Inject] private IGameDataManager _gameDataManager;
        [Inject] private ISceneGenerator _sceneGenerator;
        [Inject] private IPlayerManager _playerManager;

        /// <summary>
        /// 初始化玩家相关状态和数据
        /// </summary>
        public async Task InitPlayerAsync()
        {
            // 初始化输入系统
            using(var handle = await GameAsset.LoadAssetAsync<TextAsset>(FileUtility.InputActionLocalFileName))
            {
                _inputSystem.InitInputSystem(handle.Asset.text);
            }
            
            // 初始化游戏数据
            // TODO：先加载配置
            //await _gameDataManager.LoadDataAsync();
            // 再加载数据，数据依赖配置
            await _gameDataManager.LoadDataAsync();
            
            // 预先生成所需组件类型
            PreGenerateRequireComponentTypes();
            
            // 初始化场景
            await _sceneGenerator.InitMainScene(-1);
            // 创建玩家对象（参数为玩家配置ID，对应玩家基础配置表）
            await _playerManager.CreatePlayer(1001);
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
