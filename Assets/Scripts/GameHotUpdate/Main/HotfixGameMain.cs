using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Log;
using Core.Pool;
using Core.Reflection;
using Core.Service;
using Core.UI;
using Game.FloatingText;
using Game.Main;
using Game.Manager;
using Game.Objects;
using GameHotUpdate.Cameras;
using GameHotUpdate.Manager;
using GameHotUpdate.Objects;
using GameHotUpdate.UI.Main;
using UnityEngine;
using IGameManager = Game.Manager.IGameManager;
using Object = UnityEngine.Object;

namespace GameHotUpdate.Main
{
    /// <summary>
    /// 游戏主入口类
    /// 继承单例MonoBehaviour，保证全局唯一实例，负责游戏初始化、场景加载/清理等核心流程
    /// </summary>
    public class HotfixGameMain
    {
        /// <summary>
        /// 游戏启动入口方法
        /// </summary>
        private static async void Init()
        {
            try
            {
                // 初始化工厂
                ServiceLocator.Get<IFactoryManager>().InitFactorys();
                // 注册游戏业务层管理器到服务容器（供全局调用）
                ServiceLocator.Register<IGameManager>(GameManager.Instance);
                // 初始化游戏数据、服务
                await ServiceLocator.Get<IGameManager>().Init(new GameDataManager(), new GameServiceManger());
                // 初始化UI管理器，创建画布和UI相机
                await ServiceLocator.Get<IUIManager>().InitUIManagerAsync();
                // 自动登录逻辑（暂注释，可根据业务开启）
                //await ServiceLocator.Get<IServerManager>().TryAutoLogin();
                // 初始化游戏场景（创建NPC、玩家、UI等核心游戏对象）
                await InitScene();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(HotfixGameMain)}.{nameof(Init)}: {e.Message}");
            }
        }

        /// <summary>
        /// 初始化游戏场景核心内容
        /// 异步创建NPC、玩家对象，初始化UI界面、飘字管理器等游戏元素
        /// </summary>
        public static async Task InitScene()
        {
            // 创建村民NPC对象
            // 参数说明：资源类型（预制体）、预制体资源键、生成位置、旋转角度
            var villager = await ServiceLocator.Get<IObjectBuilder>().
                GetHotfixObject<NpcObject>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Npc, null);
            villager.transform.SetPositionAndRotation(new Vector3(0, 1, 8.39f), Quaternion.identity);
            // 初始化NPC基础属性（参数为NPC配置ID，对应配置表）
            villager.BaseInit(1);

            // 创建流浪汉NPC对象
            var Vagrant = await ServiceLocator.Get<IObjectBuilder>().
                GetHotfixObject<NpcObject>(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Npc,null);
            Vagrant.transform.SetPositionAndRotation(new Vector3(6.94f, 1, 8.39f), Quaternion.identity);
            Vagrant.BaseInit(2);
            
            // 创建玩家对象（参数为玩家配置ID，对应玩家基础配置表）
            await ServiceLocator.Get<IPlayerManager>().CreatePlayer(1001);
            
            // 创建主界面UI（MVC架构）：指定UI层级为中层，初始化MainView、MainModel、MainController
            var mainController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<MainView, MainModel, MainController>(E_UILayer.Mid, ResKeyCollection.MainView);
            
            // 初始化飘字管理器
            ServiceLocator.Get<IFloatingTextManager>().Init();
        }
        
        /// <summary>
        /// 清理游戏场景资源
        /// 场景切换/游戏退出时调用，释放所有可回收资源，避免内存泄漏
        /// </summary>
        public static void ClearScene()
        {
            // 1. 销毁轨道相机对象（场景核心相机）
            Object.Destroy(OrbitCameraController.Instance.gameObject);

            // 2. 清理玩家数据和对象（移除玩家实例、重置玩家状态）
            ServiceLocator.Get<IPlayerManager>().Clear();

            // 3. 清理飘字缓存（释放浮动文字对象池）
            ServiceLocator.Get<IFloatingTextManager>().ClearCache();

            // 4. 清空对象池（释放所有预制体缓存，如NPC、特效等）
            ServiceLocator.Get<IPoolManager>().Clear();
        }
    }
}