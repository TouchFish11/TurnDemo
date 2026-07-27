using System;
using System.Collections.Generic;
using Core.DI;
using Core.Log;
using Core.Pool;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.Base.UI;
using HotUpdate.Game.InventoryModule.Items;
using HotUpdate.UI.Main.Logic;

namespace HotUpdate.UI.Main
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 主界面控制器核心类
    /// 职责：处理主界面的业务逻辑、事件订阅/取消、按钮点击、状态初始化等
    /// 继承：UIController（基础UI控制器），实现IMainController接口
    /// </summary>
    public class MainController : UIController<MainView>, IBlockOperation
    {
        [Inject] private IPoolManager _poolManager;
        [Inject] private IUIService _uiService;
        [Inject] private ItemDataProvider _itemDataProvider;

        /// <summary>
        /// 主界面逻辑对象字典
        /// 键：逻辑类类型（如InteractLogic/TaskLogic）
        /// 值：对应逻辑类实例，用于解耦不同模块的业务逻辑
        /// </summary>
        private readonly Dictionary<Type, MainLogic> mainLogics = new();

        public bool BlockOperation { get; } = false;

        /// <summary>
        /// 控制器初始化方法
        /// 执行时机：控制器创建后自动调用
        /// 职责：订阅事件、注册回调、初始化状态
        /// </summary>
        /// <returns>异步任务</returns>
        protected override async Task OnInit()
        {
            // 初始化交互逻辑实例并加入字典
            mainLogics.Add(typeof(InteractLogic), _poolManager.GetData<InteractLogic>());
            // 初始化对话逻辑实例并加入字典
            mainLogics.Add(typeof(DialogueLogic), _poolManager.GetData<DialogueLogic>());
            // 初始化任务逻辑实例并加入字典
            mainLogics.Add(typeof(QuestLogic), _poolManager.GetData<QuestLogic>());
            // 初始化所有子逻辑模块的状态
            await InitState();
        }

        protected override Task OnActive()
        {
            // 显示主界面显示效果
            // ...
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            // 显示主界面隐藏效果
            // ...
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 按钮点击事件处理方法
        /// 执行时机：主界面按钮被点击时触发
        /// </summary>
        /// <param name="btnName">按钮名称（与UI配置中的按钮名对应）</param>
        protected override async void OnButtonClick(string btnName)
        {
            try
            {
                // 失活主界面
                await _uiService.CloseAsync(panelId, false);
                switch (btnName)
                {
                    case "btnActivity":
                        await _uiService.OpenAsync(EUIPanelId.ActivityPanel, E_UILayer.Bot);
                        break;
                    case "btnJourney":
                        
                        break;
                    case "btnBag":
                        // Test
                        _itemDataProvider.AddData(10001, 1);
                        _itemDataProvider.AddData(10002, 2);
                        _itemDataProvider.AddData(10003, 3);
                        _itemDataProvider.AddData(20001, 1);
                        _itemDataProvider.AddData(20002, 1);
                        
                        await _uiService.OpenAsync(EUIPanelId.InventoryPanel, E_UILayer.Bot);
                        break;
                    // 任务按钮点击：打开任务界面
                    case "btnTask":
                        await _uiService.OpenAsync(EUIPanelId.QuestPanel, E_UILayer.Bot);
                        break;
                    case "btnTeam":
                        
                        break;
                    case "btnRole":
                        
                        break;
                    case "btnSettings":
                        await _uiService.OpenAsync(EUIPanelId.SettingPanel, E_UILayer.Bot);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Main, e);
            }
        }
        
        /// <summary>
        /// 初始化所有子逻辑模块的状态
        /// 遍历mainLogics字典，执行每个逻辑模块的Init方法
        /// </summary>
        private async Task InitState()
        {
            foreach (var item in mainLogics.Values)
            {
                await item.Init(this, view);
            }
        }
    }
}