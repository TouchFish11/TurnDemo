using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.UI;
using HotUpdate.Base.Scene;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.TargetSelect;
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
        [Inject] private IBattleEventScheduler battleEventScheduler;
        [Inject] private IDamageCalcManager damageCalcManager;
        [Inject] private IBattleInputHandler battleInputHandler;
        [Inject] private IBattleCameraManager battleCameraManager;
        [Inject] private IBattleCommandsController battleCommandsController;
        [Inject] private ITargetSelectManager targetSelectManager;
        
        public WaveCreator WaveCreator { get; private set; }
        
        public BattleService BattleService { get; private set; }
        
        /// <summary>
        /// 战斗结束委托
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
            
            // 创建战斗上下文
            var context = DIContainer.Create<BattleContext>();
            var battleEventBus = DIContainer.Create<BattleEventBus>();
            var battleStateMachine = DIContainer.Create<BattleStateMachine>(parameterValues: context);
            context.Init(battleEventBus, battleStateMachine);
            // 监听战斗退出事件
            context.EventBus.AddListener<QuitBattleEvent>(OnQuitBattleEvent);
            
            // 初始化各个战斗管理器
            battleEventScheduler.Init(context);
            damageCalcManager.Init(context);
            battleInputHandler.Init(context);
            battleCameraManager.Init(context);
            battleCommandsController.Init(context, this);
            targetSelectManager.Init(context);
            
            // 创建战斗服务
            BattleService ??= DIContainer.Create<BattleService>();
            BattleService.Init(this, context);
            // 创建WaveCreator并初始化
            WaveCreator ??= DIContainer.Create<WaveCreator>(parameterValues: this);
            WaveCreator.Init(context, waveDatas);
            
            // 开始战斗
            context.BattleMachine.StartBattle();
        }

        public BattleService GetBattleService()
        {
            return  BattleService;
        }
        
        /// <summary>
        /// 战斗退出事件回调
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private async void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            try
            {
                var context = quitBattleEvent.Context;
                context.EventBus.RemoveListener<QuitBattleEvent>(OnQuitBattleEvent);
                // 创建黑背景界面遮挡
                var controller = await _uiService.OpenAsync(EUIPanelId.BlackBackPanel, E_UILayer.Bot);
                // 强制不可见，暂时这样处理，正常流程Bug：battleLoadingController销毁时未正确释放
                _mouseManager.ForceInVisible();
                // 销毁战斗界面
                await _uiService.CloseAsync(quitBattleEvent.BattleUIController.PanelId, true);
                // 清理战斗数据
                ClearBattle(context);
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

        /// <summary>
        /// 清理战斗数据缓存
        /// </summary>
        /// <param name="context"></param>
        private void ClearBattle(IBattleContext context)
        {
            // 销毁所有实体 GameObject
            foreach (var entity in context.AllBattleEntity)
            {
                entity.Destroy();
                EngineUtility.Destroy(entity.GameObject);
            }
            // 销毁状态机
            context.BattleMachine.Dispose();
            // 清空事件总线
            context.EventBus.Clear();
            context.CleanData();
            // 清空缓存池
            _poolManager.ClearAll();
        }
    }
}
