using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Log;
using Core.Service;
using Core.UI;
using Core.UI.MVC;
using GameHotUpdate.Activity.UI.Base;
using GameHotUpdate.Battle;
using GameHotUpdate.Battle.Turn;
using GameHotUpdate.Config;
using GameHotUpdate.Main.UI.Logic;
using GameHotUpdate.Tasks.UI;

namespace GameHotUpdate.Main.UI
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
        private readonly Dictionary<Type, MainLogic> mainLogics = new();
        
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
            // 初始化对话逻辑实例并加入字典
            mainLogics.Add(typeof(DialogueLogic), new DialogueLogic(this, model, view));
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
                    case "btnActivity":
                        await ServiceLocator.Get<IUIManager>().CreateViewAsync<ActivityView, ActivityModel, ActivityController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.ActivityView);
                        break;
                    case "btnJourney":
                        
                        break;
                    case "btnBag":
                        
                        break;
                    // 任务按钮点击：打开任务界面
                    case "btnTask":
                        await ServiceLocator.Get<IUIManager>().CreateViewAsync<TaskView, TaskModel, TaskController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.TaskView);
                        break;
                    // 战斗测试按钮点击：启动战斗
                    case "btnBattleTest":
                        var turnData = new TurnData
                        {
                            TotalTurnNumber = 1,
                            Waves = new List<List<int>>
                            {
                                new(){1,4,1},
                                // new(){1,4,1},
                            }
                        };
                        
                        // 通过BattleManager启动战斗，传入当前控制器上下文
                        await ServiceLocator.Get<IBattleManager>().EnterBattle(turnData, () =>
                        {
                            //...
                            return Task.CompletedTask;
                        });
                        break;
                    case "btnTeam":
                        
                        break;
                    case "btnRole":
                        
                        break;
                }
            }
            catch (Exception e)
            {
                // 捕获按钮点击异常，输出错误日志
                LogManager.LogError($"：{nameof(MainController)}.{nameof(ButtonOnClick)}：{e.Message}");
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
            foreach (var item in mainLogics.Values)
            {
                item.Dispose();
            }
            
            // 执行基类的Destroy方法（基础销毁逻辑）
            base.Destroy();
        }
    }
}