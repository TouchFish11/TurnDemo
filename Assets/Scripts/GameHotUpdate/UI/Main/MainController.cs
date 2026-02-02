using System;
using System.Collections.Generic;
using Core.Config;
using Core.GlobalEvent;
using Core.Log;
using Core.Service;
using Core.UI;
using Game.Battle;
using Game.Dialogue;
using Game.Interact;
using Game.Tasks;
using GameHotUpdate.UI.Main.Logic;
using GameHotUpdate.UI.MVC;
using GameHotUpdate.UI.Task;

namespace GameHotUpdate.UI.Main
{
    /// <summary>
    /// 主界面控制器核心类
    /// 职责：处理主界面的业务逻辑、事件订阅/取消、按钮点击、状态初始化等
    /// 继承：UIController（基础UI控制器），实现IMainController接口
    /// </summary>
    public class MainController : UIController<MainView, MainModel>
    {
        /// <summary>
        /// 主界面逻辑对象字典
        /// 键：逻辑类类型（如InteractLogic/TaskLogic）
        /// 值：对应逻辑类实例，用于解耦不同模块的业务逻辑
        /// </summary>
        private readonly Dictionary<Type, MainLogic> mainLogics = new Dictionary<Type, MainLogic>();
        
        /// <summary>
        /// 控制器初始化方法（异步）
        /// 执行时机：控制器创建后自动调用
        /// 职责：订阅事件、注册回调、初始化状态
        /// </summary>
        /// <returns>异步任务</returns>
        protected override System.Threading.Tasks.Task OnInit()
        {
            // 初始化交互逻辑实例并加入字典
            mainLogics.Add(typeof(InteractLogic), new InteractLogic(this, model, view));
            // 初始化任务逻辑实例并加入字典
            mainLogics.Add(typeof(TaskLogic), new TaskLogic(this, model, view));
            
            // 订阅交互事件（当触发InteractEvent时，执行OnInteractEvent回调）
            ServiceLocator.Get<IEventCenter>().SubscribeEvent<InteractEvent>(OnInteractEvent);
            
            // 注册对话系统回调：对话开始时隐藏主界面
            ServiceLocator.Get<IDialogueManager>().OnDialogueStart += InActive;
            // 注册对话系统回调：对话结束时显示主界面
            ServiceLocator.Get<IDialogueManager>().OnDialogueEnd += Active;
            
            // 注册任务系统回调：任务更新时执行TaskLogic的UpdateTask方法
            ServiceLocator.Get<ITaskManager>().OnUpdateTask += mainLogics[typeof(TaskLogic)].As<TaskLogic>().UpdateTask;
            // 注册任务系统回调：任务取消时执行TaskLogic的CancelTask方法
            ServiceLocator.Get<ITaskManager>().OnCancelTask += mainLogics[typeof(TaskLogic)].As<TaskLogic>().CancelTask;
            
            // 初始化任务栏状态：默认隐藏
            mainLogics[typeof(TaskLogic)].As<TaskLogic>().SetTaskbarActive(false);
            // 初始化所有子逻辑模块的状态
            InitState();

            return System.Threading.Tasks.Task.CompletedTask;
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
                switch (btnName)
                {
                    // 任务按钮点击：打开任务界面
                    case "btnTask":
                        // 通过UIManager创建任务界面，指定层级为Mid，资源键为TaskView
                        await ServiceLocator.Get<IUIManager>().CreateViewAsync<TaskView, TaskModel, TaskController>(E_UILayer.Mid, ResKeyCollection.TaskView);
                        break;
                    
                    // 战斗测试按钮点击：启动战斗
                    case "btnBattleTest":
                        // 通过BattleManager启动战斗，传入当前控制器上下文
                        await ServiceLocator.Get<IBattleManager>().StartBattle(this);
                        break;
                }
            }
            catch (Exception e)
            {
                // 捕获按钮点击异常，输出错误日志
                LogManager.LogError($"主界面按钮点击报错：{e.Message}，按钮名：{btnName}，异常详情：{e.StackTrace}");
            }
        }

        /// <summary>
        /// 交互事件回调方法
        /// 执行时机：接收到InteractEvent事件时触发
        /// </summary>
        /// <param name="interactEvent">交互事件数据（包含可交互对象列表）</param>
        private void OnInteractEvent(InteractEvent interactEvent)
        {
            // 调用交互逻辑层，创建交互界面/逻辑（传入可交互对象）
            mainLogics[typeof(InteractLogic)].As<InteractLogic>().CreateInteract(interactEvent.Interactables);
        }

        /// <summary>
        /// 激活主界面
        /// 作用：设置主界面为显示状态
        /// </summary>
        private void Active()
        {
            ServiceLocator.Get<IUIManager>().SetViewActive<MainController>(true);
        }

        /// <summary>
        /// 隐藏主界面
        /// 作用：设置主界面为隐藏状态
        /// </summary>
        private void InActive()
        {
            ServiceLocator.Get<IUIManager>().SetViewActive<MainController>(false);
        }

        /// <summary>
        /// 初始化所有子逻辑模块的状态
        /// 遍历mainLogics字典，执行每个逻辑模块的Init方法
        /// </summary>
        private void InitState()
        {
            foreach (var item in mainLogics.Values)
            {
                item.Init();
            }
        }

        /// <summary>
        /// 控制器销毁方法
        /// 执行时机：界面关闭/控制器被销毁时调用
        /// 职责：取消事件订阅，释放资源，防止内存泄漏
        /// </summary>
        public override void Destroy()
        {
            // 执行基类的Destroy方法（基础销毁逻辑）
            base.Destroy();
            
            // 取消交互事件的订阅
            ServiceLocator.Get<IEventCenter>().UnsubscribeEvent<InteractEvent>(OnInteractEvent);
            // 取消其他回调（如对话/任务系统）
            ServiceLocator.Get<IDialogueManager>().OnDialogueStart -= InActive;
            ServiceLocator.Get<IDialogueManager>().OnDialogueEnd -= Active;
             
            ServiceLocator.Get<ITaskManager>().OnUpdateTask -= mainLogics[typeof(TaskLogic)].As<TaskLogic>().UpdateTask;
            ServiceLocator.Get<ITaskManager>().OnCancelTask -= mainLogics[typeof(TaskLogic)].As<TaskLogic>().CancelTask;
        }
    }
}