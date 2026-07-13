using System;
using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Pool;
using Core.UI;
using HotUpdate.Base.Scene;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
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
        
        private IBattleContext _context;
        
        public WaveCreator WaveCreator { get; private set; }
        
        public BattleService BattleService { get; private set; }
        
        /// <summary>
        /// 战斗结束委托
        /// </summary>
        private Func<BattleResult, Task> OnBattleOver;
        
        public async Task Init(IBattleContext context, BattleStartupParams startupParams)
        {
            OnBattleOver = startupParams.OnBattleOver;
            // 创建战斗加载界面
            var battleLoadingController = (IBattleLoadingController)await _uiService.OpenAsync(EUIPanelId.BattleLoadingkPanel, E_UILayer.Bot);
            // 在加载界面显示后，在执行该回调
            if (startupParams.OnPreEnter != null)
            {
                await startupParams.OnPreEnter();
            }
            
            // 加载战斗场景
            await _sceneGenerator.InitSceneAsync(AssetKeys.LevelScene, LoadSceneMode.Single, battleLoadingController.UpdateProgress);
            // 隐藏主界面
            await _uiService.CloseAsync(_uiService.GetPanel(EUIPanelId.MainPanel).PanelId, false);
            
            // 创建战斗服务
            BattleService ??= DIContainer.Create<BattleService>();
            BattleService.Init(this, context);
            // 创建WaveCreator并初始化
            WaveCreator ??= DIContainer.Create<WaveCreator>(parameterValues: this);
            WaveCreator.Init(context, startupParams.WaveDatas);
            _context = context;
        }
        
        public async void QuitBattle(int battlePanelId)
        {
            try
            {
                // 创建黑背景界面遮挡
                var controller = await _uiService.OpenAsync(EUIPanelId.BlackBackPanel, E_UILayer.Bot);
                // 强制不可见，暂时这样处理，正常流程Bug：battleLoadingController销毁时未正确释放
                _mouseManager.ForceInVisible();
                // 销毁战斗界面
                await _uiService.CloseAsync(battlePanelId, true);
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
                Logger.LogException(ELogTags.Battle, e);
            }
        }

        /// <summary>
        /// 清理战斗数据缓存
        /// </summary>
        public void Reset()
        {
            
        }
    }
}
