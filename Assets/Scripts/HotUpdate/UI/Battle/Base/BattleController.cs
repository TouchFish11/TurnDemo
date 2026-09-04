using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events.ViewOperation;
using Core.Log;
using Core.Time;
using Core.UI.ViewController;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.UI;
using HotUpdate.UI.Battle.ActionLine;
using HotUpdate.UI.Battle.MonsterStateUI;

namespace HotUpdate.UI.Battle.Base
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 战斗界面控制器
    /// </summary>
    public class BattleController : UIController<BattleView>, IBattleController
    {
        [Inject] private IEventCenter _eventCenter;
        [Inject] private ITimerManager _timerManager;
        
        public IBattleUIInitializer UiInitializer { get; private set; }
        
        public IBattleEventProcessor EventProcessor { get; private set; }
        
        public IBattleUIManager BattleUiManager { get; private set; }
        
        public IMonsterStateUIManager MonsterStateUIManager { get; private set; }

        protected override bool IsCursorVisible { get; set; } = true;

        protected override Task OnInit()
        {
            // 初始化界面UI
            var actionExecuteGridLogic = DIContainer.Create<ActionExecuteGridLogic>();
            actionExecuteGridLogic.Init(view.ActionExecuteGridUI);
            view.InitActionExecuteGrid(actionExecuteGridLogic);
            view.InitActingTip();
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            view.OnClick += OnViewClick;
            view.OnDragging += OnViewDragging;
            view.OnLeftDrag += OnViewLeftDrag;
            view.OnRightDrag += OnViewRightDrag;
            view.OnRebound += OnViewRebound;
            
            UiInitializer = DIContainer.Create<BattleUIInitializer>(parameterValues: new object[] { view, this });
            BattleUiManager = DIContainer.Create<BattleUIManager>(parameterValues: new object[] { view, this });
            EventProcessor = DIContainer.Create<BattleEventProcessor>(parameterValues: new object[] { this, BattleUiManager, UiInitializer });
            MonsterStateUIManager = DIContainer.Create<MonsterStateUIManager>();
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            view.OnClick -= OnViewClick;
            view.OnDragging -= OnViewDragging;
            view.OnLeftDrag -= OnViewLeftDrag;
            view.OnRightDrag -= OnViewRightDrag;
            view.OnRebound -= OnViewRebound;
            
            UiInitializer.Dispose();
            UiInitializer = null;
            BattleUiManager.Dispose();
            BattleUiManager = null;
            EventProcessor.Dispose();
            EventProcessor = null;
            MonsterStateUIManager.Dispose();
            MonsterStateUIManager = null;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化战斗控制器
        /// </summary>
        /// <param name="battleContext"></param>
        public void InitBattleController(IBattleContext battleContext)
        {
            // 注册战斗相关事件
            EventProcessor.RegisterBattleEvents(battleContext.EventBus);
            Logger.LogDebug(ELogTags.Battle, $"Battle init controller finished");
        }

        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(view.btnClose))
            {
                view.ActionStatusArea.gameObject.SetActive(false);
                foreach (var actionGridUi in view.ActionGridUis)
                {
                    actionGridUi.SetClickSelect(false);
                }
                
                BattleUiManager.ClearActionStatus();
                _timerManager.SetTimeRate(ETimeRate.Recovery);
            }
        }

        private void OnViewClick()
        {
            var viewClickEvent = EventSource.Get<ViewClickEvent>();
            viewClickEvent.UIView = view.GetType();
            _eventCenter.TriggerEvent(viewClickEvent);
        }
        
        private void OnViewDragging(float deltaX)
        {
            var viewDraggingEvent = EventSource.Get<ViewDraggingEvent>();
            viewDraggingEvent.UIView = view.GetType();
            viewDraggingEvent.DeltaX = deltaX;
            _eventCenter.TriggerEvent(viewDraggingEvent);
        }

        private void OnViewLeftDrag()
        {
            var viewLeftDragEvent = EventSource.Get<ViewLeftDragEvent>();
            viewLeftDragEvent.UIView = view.GetType();
            _eventCenter.TriggerEvent(viewLeftDragEvent);
        }
        
        private void OnViewRightDrag()
        {
            var viewRightDragEvent = EventSource.Get<ViewRightDragEvent>();
            viewRightDragEvent.UIView = view.GetType();
            _eventCenter.TriggerEvent(viewRightDragEvent);
        }

        private void OnViewRebound(bool isRebound)
        {
            var viewReboundEvent = EventSource.Get<ViewReboundEvent>();
            viewReboundEvent.UIView = view.GetType();
            viewReboundEvent.IsRebound = isRebound;
            _eventCenter.TriggerEvent(viewReboundEvent);
        }
    }
}