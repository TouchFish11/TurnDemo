using System;
using Core.DI;
using Core.Pool;
using Core.PreLoad;
using Core.Scene;
using Core.UI;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Damage;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Input;
using HotUpdate.Base.Battle.Point;
using HotUpdate.Base.Battle.TargetSelect;
using HotUpdate.Base.Battle.Turn;
using HotUpdate.Base.Camera;
using HotUpdate.Base.Input;
using HotUpdate.Common;
using HotUpdate.Game.Battle.BattlePoint;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Input;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.Turn;
using UnityEngine;
using UnityEngine.U2D;

namespace HotUpdate.Game.Battle.Core
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : IBattleManager
    {
        private readonly IUIManager _uiManager;
        private readonly ISceneManager _sceneManager;
        private readonly IMouseManager _mouseManager;
        private readonly IPoolManager _poolManager;
        
        // 战斗上下文
        private IBattleContext _context;
        // 回合创建器
        private TurnCreator _creator;
        
        /// <summary>
        /// 战斗结束事件
        /// </summary>
        private Func<Task> OnBattleOver;

        public BattleManager(IUIManager uiManager, ISceneManager sceneManager, IMouseManager mouseManager, IPoolManager poolManager)
        {
            _uiManager = uiManager;
            _sceneManager = sceneManager;
            _mouseManager = mouseManager;
            _poolManager = poolManager;
        }

        /// <summary>
        /// 注册战斗相关管理器
        /// </summary>
        private static void RegisterManager(IBattleContext context)
        {
            DIContainer.GetInstance.Register<ITargetSelectManager>(new TargetSelectManager(context));
            // IDamageCalcManager 依赖 ITargetSelectManager
            DIContainer.GetInstance.Register<IDamageCalcManager>(new DamageCalcManager(context));
            DIContainer.GetInstance.Register<IBattleInputHandler>(new BattleInputHandler(context));
            DIContainer.GetInstance.Register<IBattleEventScheduler>(new BattleEventScheduler(context));
            //  IBattleCameraManager 依赖 IBattleInputHandler
            DIContainer.GetInstance.Register<IBattleCameraManager>(new BattleCameraManager(DIContainer.GetInstance<IPrefabLoader>()));
        }

        /// <summary>
        /// 取消战斗相关管理器的注册
        /// </summary>
        private static void UnregisterManager()
        {
            DIContainer.GetInstance.Unregister<IBattleCameraManager>();
            DIContainer.GetInstance.Unregister<ITargetSelectManager>();
            DIContainer.GetInstance.Unregister<IDamageCalcManager>();
            DIContainer.GetInstance.Unregister<IBattleInputHandler>();
            DIContainer.GetInstance.Unregister<IBattlePointProxy>();
            DIContainer.GetInstance.Unregister<IBattleEventScheduler>();
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
            var battleLoadingController = await DIContainer.GetInstance<IMainUiHelper>().CreateBattleLoadingController();
            // 在加载界面显示后，在执行该回调
            if (OnpreEnter != null)
            {
                await OnpreEnter();
            }
            
            // 加载战斗场景
            await _sceneManager.LoadSceneAsync(ResKeyCollection.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, progress => battleLoadingController.UpdateProgress(progress));
            
            // 隐藏主界面
            await _uiManager.SetViewActive(_uiManager.GetController<IMainController>(), false);
            // 注册战斗点，依赖战斗场景加载完成
            DIContainer.GetInstance.Register<IBattlePointProxy>(new BattlePointProxy());
            // 预加载资源
            await PreLoad();
            // 创建战斗上下文，依赖战斗点代理
            _context = new BattleContext(DIContainer.GetInstance<IBattlePointProxy>());
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
            
            await DIContainer.GetInstance<IPreLoadManager>().PreLoads(preLoadDatas);
        }
        
        public IBattleContext GetContext()
        {
            return _context;
        }

        public ITurnCreator GetTurnCreator()
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
                // 移除注册
                UnregisterManager();
                // 清理战斗数据
                _context.CleanupBattle();

                // 创建黑背景界面遮挡
                var backController = await DIContainer.GetInstance<IMainUiHelper>().CreateBackController();
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
