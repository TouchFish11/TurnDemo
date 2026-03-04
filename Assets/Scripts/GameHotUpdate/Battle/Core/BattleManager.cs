using System;
using Core.Log;
using Core.Pool;
using Core.PreLoad;
using Core.Scene;
using Core.Service;
using Core.Singleton;
using Core.UI;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.BattlePoint;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Damage;
using GameHotUpdate.Battle.Event;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Input;
using GameHotUpdate.Battle.Point;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Interface;
using GameHotUpdate.Battle.TargetSelect;
using GameHotUpdate.Battle.Turn;
using GameHotUpdate.Camera;
using GameHotUpdate.Config;
using GameHotUpdate.Input;
using GameHotUpdate.Main.Back;
using GameHotUpdate.Main.Loading.Battle;
using GameHotUpdate.Main.UI;
using UnityEngine;
using UnityEngine.U2D;

namespace GameHotUpdate.Battle.Core
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>, IBattleManager
    {
        private readonly IUIManager _uiManager = ServiceLocator.Get<IUIManager>();
        private readonly ISceneManager _sceneManager = ServiceLocator.Get<ISceneManager>();
        private readonly IMouseManager _mouseManager = ServiceLocator.Get<IMouseManager>();
        private readonly IPoolManager _poolManager = ServiceLocator.Get<IPoolManager>();
        
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
        /// <param name="turnData">回合数据</param>
        /// <param name="OnpreEnter">在进入前执行回调</param>
        /// <param name="onBattleOver">在结束后执行回调</param>
        public async Task EnterBattle(TurnData turnData, Func<Task> OnpreEnter, Func<Task> onBattleOver)
        {
            // 缓存回调
            OnBattleOver = onBattleOver;
            // 创建战斗加载界面
            var battleLoadingController = await _uiManager.CreateViewAsync<BattleLoadingView, BattleLoadingModel, BattleLoadingController>(AbKeyCollection.Ui, E_UILayer.Bot, ResKeyCollection.BattleLoadingView);
            // 在加载界面显示后，在执行该回调
            if (OnpreEnter != null)
            {
                await OnpreEnter();
            }
            
            // 加载战斗场景
            await _sceneManager.LoadSceneAsync(ResKeyCollection.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, progress => battleLoadingController.UpdateProgress(progress));
            
            // 隐藏主界面
            await _uiManager.SetViewActive(_uiManager.GetController<MainController>(), false);
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
            _creator = _poolManager.GetData<TurnCreator>();
            _creator.Init(_context, turnData.TotalTurnNumber, turnData.Waves);
            // 开始战斗
            _context.GetStateMachine().StartBattle();
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
                new(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Warrior, typeof(GameObject)),
                new(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Wizard, typeof(GameObject)),
                new(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Slime, typeof(GameObject)),
                new(AbKeyCollection.Prefab, ResKeyCollection.Prefab_TurtleShell, typeof(GameObject)),
                new(AbKeyCollection.Prefab, ResKeyCollection.Prefab_TurtleShell, typeof(GameObject)),
                
                // UI
                new(AbKeyCollection.Ui, ResKeyCollection.SelectMarkerUI, typeof(GameObject)),
                new(AbKeyCollection.Ui, ResKeyCollection.MonsterStateUI, typeof(GameObject)),
                new(AbKeyCollection.Ui, ResKeyCollection.RoleStateUI, typeof(GameObject)),
                new(AbKeyCollection.Ui, ResKeyCollection.ActionGridUI, typeof(GameObject)),
                new(AbKeyCollection.Ui, ResKeyCollection.WaitingActUI, typeof(GameObject)),
                new(AbKeyCollection.Ui, ResKeyCollection.SkillKeyUI, typeof(GameObject)),
                
                // SpriteAtlas
                new(AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Icon_BattleEntity, typeof(SpriteAtlas)),
                new(AbKeyCollection.Spriteatlas, ResKeyCollection.BrightIcons, typeof(SpriteAtlas)),
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
        private async void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            try
            {
                // 清理战斗数据
                _context.CleanupBattle();
            
                // 销毁战斗输入处理器、战斗点对象、战斗UI调度器
                UnityEngine.Object.Destroy(ServiceLocator.Get<IBattleCameraManager>().GameObject);
                UnityEngine.Object.Destroy(ServiceLocator.Get<IBattleInputHandler>().GameObject);
                UnityEngine.Object.Destroy(ServiceLocator.Get<IBattleEventScheduler>().GameObject);
            
                // 移除注册
                UnregisterManager();

                // 创建黑背景界面遮挡
                var backController = await _uiManager.CreateViewAsync<BackView, BackModel, BackController>(AbKeyCollection.Ui, E_UILayer.Bot, ResKeyCollection.BackView);
                // 强制不可见，暂时这样处理，正常流程Bug：battleLoadingController销毁时未正确释放
                _mouseManager.ForceInVisible();
                // 销毁战斗界面
                _uiManager.DestroyView(AbKeyCollection.Ui, quitBattleEvent.BattleUIController);
                // 执行战斗结束回调，在背景界面销毁前执行
                if (OnBattleOver != null)
                {
                    await OnBattleOver();
                    OnBattleOver = null;
                    // 销毁黑背景界面
                    _uiManager.DestroyView(AbKeyCollection.Ui, backController);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(BattleManager)}.{nameof(OnQuitBattleEvent)}：{e.Message}，{e.StackTrace}");
            }
        }
    }
}
