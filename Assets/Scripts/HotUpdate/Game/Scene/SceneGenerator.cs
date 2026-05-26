using Core.AssetBundles.Management;
using Core.DI;
using Core.Pool;
using HotUpdate.Base.Scene;
using HotUpdate.Common.Generated;
using HotUpdate.Game.Cameras;
using HotUpdate.Game.Interact;
using HotUpdate.Game.Main;
using HotUpdate.Game.Main.FloatingText;
using UnityEngine;

namespace HotUpdate.Game.Scene
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 场景生成器
    /// </summary>
    public class SceneGenerator : ISceneGenerator
    {
        [Inject] private OrbitCameraController _orbitCameraController;
        [Inject] private IPlayerManager _playerManager;
        [Inject] private IFloatingTextManager _floatingTextManager;
        [Inject] private IPoolManager _poolManager;
        
        private static readonly ObjectSpawner _objectSpawner;
        
        /// <summary>
        /// 初始化主游戏场景核心内容
        /// 异步创建NPC、玩家对象，初始化UI界面、飘字管理器等游戏元素
        /// </summary>
        public async Task InitMainScene()
        {
            // 创建村民NPC对象
            var villager = await _objectSpawner.SpawnAsync<NpcObject>(AssetKeys.Prefab_Npc);
            villager.Obj.Transform.SetPositionAndRotation(new Vector3(0, 1, 8.39f), Quaternion.identity);
            // 初始化NPC基础属性（参数为NPC配置ID，对应配置表）
            villager.Obj.InitNpc(1);

            // 创建流浪汉NPC对象
            var Vagrant = await _objectSpawner.SpawnAsync<NpcObject>(AssetKeys.Prefab_Npc);
            Vagrant.Obj.Transform.SetPositionAndRotation(new Vector3(6.94f, 1, 8.39f), Quaternion.identity);
            Vagrant.Obj.InitNpc(2);
        }
        
        /// <summary>
        /// 清理主游戏场景
        /// </summary>
        public void ClearMainScene()
        {
            // 销毁相机对象
            Object.Destroy(_orbitCameraController.Transform.gameObject);
            // 清理玩家数据和对象
            _playerManager.Clear();
            // 清理飘字缓存
            _floatingTextManager.ClearCache();
            // 清空对象池
            _poolManager.ClearAll();
        }
    }
}
