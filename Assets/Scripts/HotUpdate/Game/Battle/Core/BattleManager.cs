using System;
using Core.DI;
using Core.Pool;
using Core.PreLoad;
using Core.Scene;
using Core.UI;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Inputs;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Core
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : IBattleManager
    {
        [Inject] private ISceneManager _sceneManager;
        [Inject] private IMouseManager _mouseManager;
        [Inject] private IPoolManager _poolManager;
        [Inject] private IUIService _uiService;
        
        // 战斗上下文
        private IBattleContext _context;
        // 回合创建器
        private TurnCreator _creator;
        
        /// <summary>
        /// 战斗结束事件
        /// </summary>
        private Func<Task> OnBattleOver;
        
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
            var battleLoadingController = (IBattleLoadingController)await _uiService.OpenAsync(EUIPanelId.BattleLoadingkPanel, E_UILayer.Bot);
            // 在加载界面显示后，在执行该回调
            if (OnpreEnter != null)
            {
                await OnpreEnter();
            }
            
            // 加载战斗场景
            await _sceneManager.LoadSceneAsync(AssetKeys.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, battleLoadingController.UpdateProgress);
            // 隐藏主界面
            await _uiService.CloseAsync(_uiService.GetPanel(EUIPanelId.MainPanel).PanelId, false);
            // 预加载资源
            await PreLoad();
            // 创建战斗上下文，依赖战斗点代理
            _context = DIContainer.Create<BattleContext>();
            // 监听战斗退出事件
            _context.GetEventBus().AddListener<QuitBattleEvent>(OnQuitBattleEvent);
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
                new(AssetKeys.Prefab_Warrior),
                new(AssetKeys.Prefab_Wizard),
                new(AssetKeys.Prefab_Slime),
                new(AssetKeys.Prefab_TurtleShell),
                new(AssetKeys.Prefab_TurtleShell),
                
                // UI
                new(AssetKeys.SelectMarkerUI),
                new(AssetKeys.MonsterStateUI),
                new(AssetKeys.RoleStateUI),
                new(AssetKeys.ActionGridUI),
                new(AssetKeys.WaitingActUI),
                new(AssetKeys.SkillKeyUI),
                
                // SpriteAtlas
                new(AssetKeys.Atlas_Icon_BattleEntity),
                new(AssetKeys.Atlas_Icon_Common),
                new(AssetKeys.Atlas_Default),
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
                // 清理战斗数据
                _context.CleanupBattle();

                // 创建黑背景界面遮挡
                var controller = await _uiService.OpenAsync(EUIPanelId.BlackBackPanel, E_UILayer.Bot);
                // 强制不可见，暂时这样处理，正常流程Bug：battleLoadingController销毁时未正确释放
                _mouseManager.ForceInVisible();
                // 销毁战斗界面
                await _uiService.CloseAsync(quitBattleEvent.BattleUIController.PanelId, true);
                // 执行战斗结束回调，在背景界面销毁前执行
                if (OnBattleOver != null)
                {
                    await OnBattleOver();
                    OnBattleOver = null;
                    // 销毁黑背景界面
                    await _uiService.CloseAsync(controller.PanelId, true);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(BattleManager)}.{nameof(OnQuitBattleEvent)}：{e.Message}，{e.StackTrace}");
            }
        }
    }
}
