using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.DataPersistence.Binary;
using Core.Log;
using Core.Mono;
using Core.PreLoad;
using Core.Scene;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.UI.MVC;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Event;
using Game.Battle.Input;
using Game.Battle.Objects;
using Game.Battle.Skill.Interface;
using Game.Battle.TargetSelect;
using Game.Input;
using Game.UI.Battle;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.BattlePoint;
using GameHotUpdate.Battle.Damage;
using GameHotUpdate.Battle.Event;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Context;
using GameHotUpdate.Input;
using GameHotUpdate.Main;
using GameHotUpdate.Objects;
using GameHotUpdate.TargetSelect;
using GameHotUpdate.UI.Back;
using GameHotUpdate.UI.Loading.Battle;
using UnityEngine;
using UnityEngine.U2D;

namespace GameHotUpdate.Manager
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>, IBattleManager
    {
        // 战斗上下文
        private IBattleContext _context;
        
        // 怪物创建数量，测试（1~5）
        private const int monsterCount = 5;
        
        private BattleManager()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private static void RegisterManager(IBattleContext context)
        {
            ServiceLocator.Register<ITargetSelectManager>(TargetSelectManager.Instance);
            ServiceLocator.Get<ITargetSelectManager>().Init(context);
            
            // IDamageCalcManager 依赖 ITargetSelectManager
            ServiceLocator.Register<IDamageCalcManager>(DamageCalcManager.Instance);
            ServiceLocator.Get<IDamageCalcManager>().Init(context);
            
            ServiceLocator.Register<IBattleInputHandler>(BattleInputHandler.Instance);
            ServiceLocator.Get<IBattleInputHandler>().Init(context);
            
            ServiceLocator.Register<ISkillManager>(SkillManager.Instance);
            ServiceLocator.Register<IAnimationPlayManager>(AnimationPlayManager.Instance);
            
            // 测试
            ServiceLocator.Register<IBattleEventScheduler>(BattleEventScheduler.Instance);
            ServiceLocator.Get<IBattleEventScheduler>().Init(context);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void UnregisterManager()
        {
            ServiceLocator.Unregister<ITargetSelectManager>();
            ServiceLocator.Unregister<IDamageCalcManager>();
            ServiceLocator.Unregister<ISkillManager>();
            ServiceLocator.Unregister<IAnimationPlayManager>();
            ServiceLocator.Unregister<IBattleInputHandler>();
            ServiceLocator.Unregister<IBattleUIScheduler>();
            ServiceLocator.Unregister<IBattlePointProxy>();
            ServiceLocator.Unregister<IBattleEventScheduler>();
        }

        public async Task StartBattle(IuiController controller)
        {
            // 创建战斗加载界面
            var battleLoadingController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BattleLoadingView, BattleLoadingModel, BattleLoadingController>(E_UILayer.Bot, ResKeyCollection.BattleLoadingView);
            
            // 清理场景内容缓存
            HotfixGameMain.ClearScene();
            
            // 加载战斗场景
            ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, 
            (progress) => battleLoadingController.UpdateProgress(progress), 
            async () =>
            {
                // 预加载资源
                await PreLoad();
                // 销毁主界面
                ServiceLocator.Get<IUIManager>().DestroyView(controller);
                // 注册战斗点，依赖战斗场景加载完成
                ServiceLocator.Register<IBattlePointProxy>(new BattlePointProxy());
                // 创建战斗上下文，依赖战斗点代理
                _context = new BattleContext(ServiceLocator.Get<IBattlePointProxy>());
                // 监听战斗退出事件
                _context.GetEventBus().AddListener<QuitBattleEvent>(OnQuitBattleEvent);

                // 注册管理器
                RegisterManager(_context);
                // 创建战斗实体对象，依赖战斗上下文、战斗点
                await CreateBattleEntity();
                // 初始化战斗点，依赖战斗实体对象创建完成
                ServiceLocator.Get<IBattlePointProxy>().InitProxy(_context, new List<IBattleEntityObject>(_context.GetAlivePlayerEntitys()));
                
                // 进入战斗准备
                await _context.GetTurnManager().BattlePreparation();
                // 战斗准备完毕，销毁战斗加载界面
                ServiceLocator.Get<IUIManager>().DestroyView(battleLoadingController);
                // 开始战斗
                ServiceLocator.Get<IMonoAdapter>().StartCoroutine(_context.GetTurnManager().StartBattle());
            });
        }

        /// <summary>
        /// 战斗资源预加载
        /// </summary>
        private static async Task PreLoad()
        {
            // TODO：暂时写死
            var preLoadDatas = new PreLoadData[]
            {
                // GameObject
                new(EAssetBundleType.Prefab, ResKeyCollection.Prefab_FireFly, typeof(GameObject)),
                new(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Herta, typeof(GameObject)),
                new(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Slime, typeof(GameObject)),
                new(EAssetBundleType.Prefab, ResKeyCollection.Prefab_TurtleShell, typeof(GameObject)),
                new(EAssetBundleType.Prefab, ResKeyCollection.Prefab_TurtleShell, typeof(GameObject)),
                
                // UI
                new(EAssetBundleType.UI, ResKeyCollection.SelectMarkerUI, typeof(GameObject)),
                new(EAssetBundleType.UI, ResKeyCollection.MonsterStateUI, typeof(GameObject)),
                new(EAssetBundleType.UI, ResKeyCollection.RoleStateUI, typeof(GameObject)),
                new(EAssetBundleType.UI, ResKeyCollection.ActionGridUI, typeof(GameObject)),
                new(EAssetBundleType.UI, ResKeyCollection.WaitingActUI, typeof(GameObject)),
                new(EAssetBundleType.UI, ResKeyCollection.SkillKeyUI, typeof(GameObject)),
                
                // SpriteAtlas
                new(EAssetBundleType.SpriteAtlas, ResKeyCollection.Atlas_Icon, typeof(SpriteAtlas)),
                new(EAssetBundleType.SpriteAtlas, ResKeyCollection.BrightIcons, typeof(SpriteAtlas)),
            };
            
            await ServiceLocator.Get<IPreLoadManager>().PreLoads(preLoadDatas);
        }
        
        /// <summary>
        /// TODO：可优化为使用战斗实体创建器来创建怪物、波次
        /// 创建战斗实体对象
        /// </summary>
        /// <returns></returns>
        private async Task CreateBattleEntity()
        {
            var playerTrans = new List<Transform>(ServiceLocator.Get<IBattlePointProxy>().BattlePoint.GetRoleTransforms());
            // 批量创建玩家角色（从配置+预制体）
            var playerDataDic = ServiceLocator.Get<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Editor).dataDic;
            var index = 0;
            foreach (var roleId in playerDataDic.Keys)
            {
                if (index == playerTrans.Count)
                {
                    break;
                }

                var transform = playerTrans[index];

                var hotfixPlayerObject = await RoleBuilder.CreateRole(roleId, transform);
                // 注入上下文，供角色内部组件使用
                hotfixPlayerObject.BattleInit(roleId, _context);
                // 记录角色所在的位置索引
                hotfixPlayerObject.EntityPosIndex = index;
                // 设置角色层级
                SetLayerRecursively(hotfixPlayerObject.GameObject, ServiceLocator.Get<IBattlePointProxy>().GetRoleLayer(index));
                _context.AddBattleEntity(hotfixPlayerObject);
                _context.AddSceneRole(hotfixPlayerObject);
                LogManager.Log($"已缓存场景对象：{hotfixPlayerObject}");
                index++;
            }

            // 批量创建怪物角色（从配置+预制体）
            var monsterTrans = new List<Transform>(ServiceLocator.Get<IBattlePointProxy>().BattlePoint.GetMonsterTransforms());
            var keys = new List<int>(ServiceLocator.Get<IBinaryDataManager>().GetConfig<MonsterInfoContainer>(EConfigLoadType.Editor).dataDic.Keys);
            index = 0;
            while (index < monsterCount)
            {
                var transform = monsterTrans[index];
                var monsterId = keys[Random.Range(0, keys.Count)];
                var hotfixMonsterObject = await MonsterBuilder.CreateMonster(monsterId, transform);
                hotfixMonsterObject.GameObject.name = $"{hotfixMonsterObject.GameObject.name}_{index}";
                // 注入上下文，供角色内部组件使用
                hotfixMonsterObject.BattleInit(monsterId, _context);
                // 记录怪物所在的位置索引
                hotfixMonsterObject.EntityPosIndex = index;
                _context.AddBattleEntity(hotfixMonsterObject);
                _context.AddSceneMonster(hotfixMonsterObject);
                LogManager.Log($"已缓存场景对象：{hotfixMonsterObject}");
                index++;
            }
        }
        
        /// <summary>
        /// 递归设置物体及其所有子物体的 Layer
        /// </summary>
        private static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
        
        public IBattleContext GetContext()
        {
            return _context;
        }

        /// <summary>
        /// 战斗退出事件回调
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            // 销毁战斗输入处理器、战斗点对象、战斗UI调度器
            ServiceLocator.Get<IBattlePointProxy>().Dispose();
            Object.Destroy(ServiceLocator.Get<IBattleInputHandler>().GameObject);
            Object.Destroy(ServiceLocator.Get<IBattleUIScheduler>().GameObject);
            Object.Destroy(ServiceLocator.Get<IBattleEventScheduler>().GameObject);
            
            // 移除注册
            UnregisterManager();
            // 清理战斗数据
            _context.CleanupBattle();
            // 销毁战斗界面
            ServiceLocator.Get<IUIManager>().DestroyView(quitBattleEvent.BattleUIController);
            BackMain();
        }

        /// <summary>
        /// 回到主场景
        /// </summary>
        private static async void BackMain()
        {
            // 创建黑背景界面遮挡
            var backController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BackView, BackModel, BackController>(E_UILayer.Bot, ResKeyCollection.BackView);
            
            ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, null, 
            async () =>
            {
                // 初始化场景
                await HotfixGameMain.InitScene();
                // 销毁黑背景界面
                ServiceLocator.Get<IUIManager>().DestroyView(backController);
                // 强制不可见，暂时这样处理，正常流程Bug：battleLoadingController销毁时未正确释放
                ServiceLocator.Get<IMouseManager>().ForceInVisible();
                await Task.CompletedTask;
            });
        }
    }
}
