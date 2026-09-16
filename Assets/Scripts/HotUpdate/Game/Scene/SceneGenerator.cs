using System;
using System.Threading.Tasks;
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
    public class SceneGenerator : ISceneGenerator
    {
        [Inject] private IPlayerManager _playerManager;
        [Inject] private IFloatingTextManager _floatingTextManager;
        [Inject] private IPoolManager _poolManager;
        [Inject] private ISceneManager _sceneManager;
        [Inject] private NpcFactory _npcFactory;
        
        public async Task InitSceneAsync(string sceneId, LoadSceneMode mode, Action<float> onLoadProgress, object sceneConfig = null)
        {
            await _sceneManager.LoadSceneAsync(sceneId, mode, onLoadProgress);
            
            // TODO：可根据场景配置sceneConfig动态创建场景环境
            // ...
        }

        /// <summary>
        /// 初始化主游戏场景核心内容
        /// 异步创建NPC、玩家对象，初始化UI界面、飘字管理器等游戏元素
        /// </summary>
        /// <param name="sceneId"></param>
        public async Task InitMainScene(int sceneId)
        {
            await _sceneManager.LoadSceneAsync(AssetKeys.MainScene, LoadSceneMode.Single, null);
            // 创建玩家对象（参数为玩家配置ID，对应玩家基础配置表）
            await _playerManager.CreatePlayer(1001);
            // 创建村民NPC对象
            await _npcFactory.CreateNpc(1, new Vector3(0, 0, 8.39f), Quaternion.AngleAxis(180, Vector3.up));
            // 创建流浪汉NPC对象
            await _npcFactory.CreateNpc(2, new Vector3(6.94f, 0, 8.39f), Quaternion.AngleAxis(180, Vector3.up));
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
        
        public void Reset()
        {
            
        }
    }
}
