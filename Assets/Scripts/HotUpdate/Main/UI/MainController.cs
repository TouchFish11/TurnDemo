using System;
using System.Collections.Generic;
using Core.Log;
using Core.Service;
using Core.UI;
using Core.UI.MVC;
using HotUpdate.Common;
using HotUpdate.Core.UI.Helper;
using HotUpdate.Core.UI.MVC;
using HotUpdate.Main.Settings.UI;
using HotUpdate.Main.UI.Logic;

namespace HotUpdate.Main.UI
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 主界面控制器核心类
    /// 职责：处理主界面的业务逻辑、事件订阅/取消、按钮点击、状态初始化等
    /// 继承：UIController（基础UI控制器），实现IMainController接口
    /// </summary>
    public class MainController : UIController<MainView, MainModel>, IMainController
    {
        private readonly ITaskUiHelper _taskUiHelper = ServiceLocator.Get<ITaskUiHelper>();
        private readonly IActivityUiHelper _activityUiHelper = ServiceLocator.Get<IActivityUiHelper>();
        
        
        /// <summary>
        /// 主界面逻辑对象字典
        /// 键：逻辑类类型（如InteractLogic/TaskLogic）
        /// 值：对应逻辑类实例，用于解耦不同模块的业务逻辑
        /// </summary>
        private readonly Dictionary<Type, MainLogic> mainLogics = new();

        protected override Task OnShow()
        {
            // 显示主界面显示效果
            // ...
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 控制器初始化方法
        /// 执行时机：控制器创建后自动调用
        /// 职责：订阅事件、注册回调、初始化状态
        /// </summary>
        /// <returns>异步任务</returns>
        protected override Task OnInit()
        {
            // 初始化交互逻辑实例并加入字典
            mainLogics.Add(typeof(InteractLogic), poolManager.GetData<InteractLogic>());
            // 初始化任务逻辑实例并加入字典
            mainLogics.Add(typeof(TaskLogic), poolManager.GetData<TaskLogic>());
            // 初始化对话逻辑实例并加入字典
            mainLogics.Add(typeof(DialogueLogic), poolManager.GetData<DialogueLogic>());
            // 初始化所有子逻辑模块的状态
            InitState();
            return Task.FromResult(Task.CompletedTask);
        }
        
        protected override Task OnHide()
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
        protected override async void ButtonOnClick(string btnName)
        {
            try
            {
                // 失活主界面
                await uiManager.SetViewActive(this, false);
                switch (btnName)
                {
                    case "btnActivity":
                        await _activityUiHelper.CreateActivityController();
                        break;
                    case "btnJourney":
                        
                        break;
                    case "btnBag":
                        
                        break;
                    // 任务按钮点击：打开任务界面
                    case "btnTask":
                        await _taskUiHelper.CreateTaskController();
                        break;
                    case "btnTeam":
                        
                        break;
                    case "btnRole":
                        
                        break;
                    case "btnSettings":
                        await uiManager.CreateViewAsync<SettingsView,  SettingsModel, SettingsController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.SettingsView);
                        break;
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"：{nameof(MainController)}.{nameof(ButtonOnClick)}：{e.Message}，{e.StackTrace}");
            }
        }
        
        /// <summary>
        /// 初始化所有子逻辑模块的状态
        /// 遍历mainLogics字典，执行每个逻辑模块的Init方法
        /// </summary>
        private void InitState()
        {
            foreach (var item in mainLogics.Values)
            {
                item.Init(this, model, view);
            }
        }
    }
}