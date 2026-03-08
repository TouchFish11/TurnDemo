using Core.Loader.Object;
using Core.Pool;
using Core.Service;
using HotUpdate.Common;
using HotUpdate.Core.Camera;
using HotUpdate.Core.Interact;
using HotUpdate.Core.Main;
using HotUpdate.Core.Scene;
using UnityEngine;

namespace HotUpdate.Main
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 场景生成器
    /// </summary>
    public class SceneGenerator : ISceneGenerator
    {
        private static readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        
        /// <summary>
        /// 初始化主游戏场景核心内容
        /// 异步创建NPC、玩家对象，初始化UI界面、飘字管理器等游戏元素
        /// </summary>
        public async Task InitMainScene()
        {
            // 创建村民NPC对象
            var villager = await _prefabLoader.GetObjectAsync<INpcObject>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Npc, null);
            villager.Transform.SetPositionAndRotation(new Vector3(0, 1, 8.39f), Quaternion.identity);
            // 初始化NPC基础属性（参数为NPC配置ID，对应配置表）
            villager.InitNpc(1);

            // 创建流浪汉NPC对象
            var Vagrant = await _prefabLoader.GetObjectAsync<INpcObject>(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Npc,null);
            Vagrant.Transform.SetPositionAndRotation(new Vector3(6.94f, 1, 8.39f), Quaternion.identity);
            Vagrant.InitNpc(2);
        }
        
        /// <summary>
        /// 清理主游戏场景
        /// </summary>
        public void ClearMainScene()
        {
            // 销毁相机对象
            Object.Destroy(ServiceLocator.Get<IOrbitCameraGeter>().OrbitCameraController.Transform.gameObject);
            // 清理玩家数据和对象
            ServiceLocator.Get<IPlayerManager>().Clear();
            // 清理飘字缓存
            ServiceLocator.Get<IFloatingTextManager>().ClearCache();
            // 清空对象池
            ServiceLocator.Get<IPoolManager>().Clear();
        }
    }
}
