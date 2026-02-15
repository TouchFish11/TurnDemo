using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Pool;
using Core.PreLoad;
using Core.Scene;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Event;
using Game.Battle.Input;
using Game.Battle.Skill.Interface;
using Game.Battle.TargetSelect;
using Game.Input;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.BattlePoint;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Damage;
using GameHotUpdate.Battle.Event;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Input;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.TargetSelect;
using GameHotUpdate.Battle.Turn;
using GameHotUpdate.Cameras;
using GameHotUpdate.Main;
using GameHotUpdate.Main.UI;
using GameHotUpdate.UI.Back;
using GameHotUpdate.UI.Loading.Battle;
using UnityEngine;
using UnityEngine.U2D;

namespace GameHotUpdate.Battle
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>, IBattleManager
    {
        // 战斗上下文
        private IBattleContext _context;
        // 回合创建器
        private TurnCreator _creator;
        
        /// <summary>
        /// 战斗结束事件
        /// </summary>
        private Func<Task> OnBattleOver;
        
        private BattleManager()
        {

        }

        /// <summary>
        /// 注册战斗相关管理器
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
            
            ServiceLocator.Register<IBattleEventScheduler>(BattleEventScheduler.Instance);
            ServiceLocator.Get<IBattleEventScheduler>().Init(context);
            
            //  IBattleCameraManager 依赖 IBattleInputHandler
            ServiceLocator.Register<IBattleCameraManager>(BattleCameraManager.Instance);
        }

        /// <summary>
        /// 取消战斗相关管理器的注册
        /// </summary>
        private static void UnregisterManager()
        {
            ServiceLocator.Unregister<IBattleCameraManager>();
            ServiceLocator.Unregister<ITargetSelectManager>();
            ServiceLocator.Unregister<IDamageCalcManager>();
            ServiceLocator.Unregister<ISkillManager>();
            ServiceLocator.Unregister<IAnimationPlayManager>();
            ServiceLocator.Unregister<IBattleInputHandler>();
            ServiceLocator.Unregister<IBattlePointProxy>();
            ServiceLocator.Unregister<IBattleEventScheduler>();
        }
        
        /// <summary>
        /// 进入战斗
        /// 唯一入口
        /// </summary>
        /// <param name="turnData"></param>
        /// <param name="onBattleOver"></param>
        public async Task EnterBattle(TurnData turnData, Func<Task> onBattleOver)
        {
            // 缓存回调
            this.OnBattleOver = onBattleOver;
            // 创建战斗加载界面
            var battleLoadingController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BattleLoadingView, BattleLoadingModel, BattleLoadingController>(E_UILayer.Bot, ResKeyCollection.BattleLoadingView);
            // 清理场景内容缓存
            HotfixGameMain.ClearScene();
            // 加载战斗场景
            ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, 
            (progress) => battleLoadingController.UpdateProgress(progress), 
            async () =>
            {
                // 隐藏主界面
                var controller = ServiceLocator.Get<IUIManager>().GetController<MainController>();
                ServiceLocator.Get<IUIManager>().SetViewActive(controller, false);
                // 注册战斗点，依赖战斗场景加载完成
                ServiceLocator.Register<IBattlePointProxy>(new BattlePointProxy());
                // 预加载资源
                await PreLoad();
                // 创建战斗上下文，依赖战斗点代理
                _context = new BattleContext(ServiceLocator.Get<IBattlePointProxy>());
                // 监听战斗退出事件
                _context.GetEventBus().AddListener<QuitBattleEvent>(OnQuitBattleEvent);
                // 注册战斗相关管理器
                RegisterManager(_context);
                // 创建回合创建器
                _creator = ServiceLocator.Get<IPoolManager>().GetData<TurnCreator>();
                _creator.Init(_context, turnData.TotalTurnNumber, turnData.Waves);
                // 开始战斗
                _context.GetStateMachine().StartBattle();
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
                new(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Warrior, typeof(GameObject)),
                new(EAssetBundleType.Prefab, ResKeyCollection.Prefab_Wizard, typeof(GameObject)),
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
                new(EAssetBundleType.SpriteAtlas, ResKeyCollection.Atlas_Icon_BattleEntity, typeof(SpriteAtlas)),
                new(EAssetBundleType.SpriteAtlas, ResKeyCollection.BrightIcons, typeof(SpriteAtlas)),
            };
            
            await ServiceLocator.Get<IPreLoadManager>().PreLoads(preLoadDatas);
        }
        
        public IBattleContext GetContext()
        {
            return _context;
        }

        public TurnCreator GetTurnCreator()
        {
            return _creator;
        }
        
        /// <summary>
        /// 战斗退出事件回调
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            // 清理战斗数据
            _context.CleanupBattle();
            
            // 销毁战斗输入处理器、战斗点对象、战斗UI调度器
            UnityEngine.Object.Destroy(ServiceLocator.Get<IBattleCameraManager>().GameObject);
            UnityEngine.Object.Destroy(ServiceLocator.Get<IBattleInputHandler>().GameObject);
            UnityEngine.Object.Destroy(ServiceLocator.Get<IBattleEventScheduler>().GameObject);
            
            // 移除注册
            UnregisterManager();

            // 销毁战斗界面
            ServiceLocator.Get<IUIManager>().DestroyView(quitBattleEvent.BattleUIController);
            // 回到主场景
            BackMain();
        }

        /// <summary>
        /// 回到主场景
        /// </summary>
        private async void BackMain()
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
                
                // 激活主界面
                var controller = ServiceLocator.Get<IUIManager>().GetController<MainController>();
                ServiceLocator.Get<IUIManager>().SetViewActive(controller, true);
                
                // 执行战斗结束回调
                await OnBattleOver?.Invoke();
                OnBattleOver = null;
            });
        }
    }
}
