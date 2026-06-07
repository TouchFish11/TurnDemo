using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Pool;
using Core.Scene;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Scene;
using HotUpdate.Game.Interact;
using HotUpdate.Game.Main.FloatingText;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotUpdate.Game.Scene
{
    /// <summary>
    /// 场景生成器
    /// </summary>
    public class SceneGenerator : ISceneGenerator, IDisposable
    {
        [Inject] private IPlayerManager _playerManager;
        [Inject] private IFloatingTextManager _floatingTextManager;
        [Inject] private IPoolManager _poolManager;
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private ISceneManager _sceneManager;

        /// <summary>
        /// 初始化主游戏场景核心内容
        /// 异步创建NPC、玩家对象，初始化UI界面、飘字管理器等游戏元素
        /// </summary>
        /// <param name="sceneId"></param>
        public async Task InitMainScene(int sceneId)
        {
            await _sceneManager.LoadSceneAsync(AssetKeys.MainScene, LoadSceneMode.Single, null);
            
            // 创建村民NPC对象
            var villager = await _objectSpawner.SpawnAsync<NpcObject>(AssetKeys.Prefab_Npc);
            villager.Transform.SetPositionAndRotation(new Vector3(0, 1, 8.39f), Quaternion.identity);
            // 初始化NPC基础属性（参数为NPC配置ID，对应配置表）
            villager.InitNpc(1);

            // 创建流浪汉NPC对象
            var Vagrant = await _objectSpawner.SpawnAsync<NpcObject>(AssetKeys.Prefab_Npc);
            Vagrant.Transform.SetPositionAndRotation(new Vector3(6.94f, 1, 8.39f), Quaternion.identity);
            Vagrant.InitNpc(2);
        }
        
        /// <summary>
        /// 清理主游戏场景
        /// </summary>
        public void ClearMainScene()
        {
            // 清理玩家数据和对象
            _playerManager.Clear();
            // 清理飘字缓存
            _floatingTextManager.ClearCache();
            // 清空对象池
            _poolManager.ClearAll();
        }

        public void Dispose()
        {
            _playerManager = null;
            _floatingTextManager = null;
            _poolManager = null;
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}
