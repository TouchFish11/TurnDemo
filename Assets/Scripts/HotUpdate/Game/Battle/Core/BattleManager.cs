using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.Pool;
using Core.UI;
using HotUpdate.Base.Scene;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Inputs;
using UnityEngine.SceneManagement;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : IBattleManager
    {
        [Inject] private ISceneGenerator _sceneGenerator;
        [Inject] private IMouseManager _mouseManager;
        [Inject] private IPoolManager _poolManager;
        [Inject] private IUIService _uiService;
        
        // 战斗上下文
        private IBattleContext _context;
        // 回合创建器
        private WaveCreator _creator;
        // 战斗服务对象
        private BattleService _battleService;
        
        /// <summary>
        /// 战斗结束事件
        /// </summary>
        private Func<BattleResult, Task> OnBattleOver;
        
        /// <summary>
        /// 进入战斗唯一入口
        /// </summary>
        /// <param name="waveDatas">波次数据列表</param>
        /// <param name="OnpreEnter">在进入前执行回调，一般用于清理当前场景，UI和资源预加载</param>
        /// <param name="onBattleOver">在结束后执行回调，一般用于恢复场景、UI逻辑</param>
        public async Task EnterBattle(List<WaveData> waveDatas, Func<Task> OnpreEnter, Func<BattleResult, Task> onBattleOver)
        {
            _creator ??= DIContainer.Create<WaveCreator>(parameterValues: this);
            
            // 缓存战斗结束回调
            OnBattleOver = onBattleOver;
            // 创建战斗加载界面
            var battleLoadingController = (IBattleLoadingController)await _uiService.OpenAsync(EUIPanelId.BattleLoadingkPanel, E_UILayer.Bot);
            // 在加载界面显示后，在执行该回调
            if (OnpreEnter != null)
            {
                await OnpreEnter();
            }
            
            // 加载战斗场景
            await _sceneGenerator.InitSceneAsync(AssetKeys.LevelScene, LoadSceneMode.Single, battleLoadingController.UpdateProgress);
            // 隐藏主界面
            await _uiService.CloseAsync(_uiService.GetPanel(EUIPanelId.MainPanel).PanelId, false);
            // 创建战斗上下文，依赖战斗点代理
            _context = DIContainer.Create<BattleContext>();
            _context.InitStateMachine();
            DIContainer.Create<BattleEventScheduler>(parameterValues: _context);
            // 监听战斗退出事件
            _context.GetEventBus().AddListener<QuitBattleEvent>(OnQuitBattleEvent);
            _battleService ??= DIContainer.Create<BattleService>(parameterValues: new object[] { this, _context });
            // 重新初始化
            _creator.Init(_context, waveDatas);
            // 开始战斗
            _context.GetStateMachine().StartBattle();
        }
        
        public IBattleContext GetContext()
        {
            return _context;
        }

        public IWaveCreator GetWaveCreator()
        {
            return _creator;
        }

        public BattleService GetBattleService()
        {
            return  _battleService;
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
                    await OnBattleOver(new BattleResult { IsWin = false });
                    OnBattleOver = null;
                    // 销毁黑背景界面
                    await _uiService.CloseAsync(controller.PanelId, true);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(BattleManager)}: Battle quit error,{e.Message}");
            }
        }
    }
}
