using Framework;
using Game.Battle;
using Game.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

/// <summary>
/// 主界面控制器工厂
/// </summary>
public class MainControllerFactory : UIControllerFactory<MainView, MainModel, MainController>
{
    public override MainController CreateController(MainView view, MainModel model)
    {
        return new MainController(view, model);
    }

    public override MainModel CreateModel()
    {
        return new MainModel();
    }
}

/// <summary>
/// 主界面控制器
/// </summary>
public class MainController : UIController<MainView, MainModel>
{
    private readonly Dictionary<Type, MainLogic> mainLogics = new Dictionary<Type, MainLogic>();

    public MainController(MainView view, MainModel model) : base(view, model)
    {
        mainLogics.Add(typeof(InteractLogic), new InteractLogic(this, model, view));
        mainLogics.Add(typeof(TaskLogic), new TaskLogic(this, model, view));
    }

    protected override async Task OnInit()
    {
        // 交互逻辑监听
        EventCenter.Instance.AddEventListener<List<IInteractable>>(E_EventType.E_OnInteract, mainLogics[typeof(InteractLogic)].As<InteractLogic>().CreateInteract);
        // 对话事件监听
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueStart += InActive;
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueEnd += Active;
        // 任务事件监听
        TaskManager.Instance.OnUpdateTask += mainLogics[typeof(TaskLogic)].As<TaskLogic>().UpdateTask;
        TaskManager.Instance.OnCancelTask += mainLogics[typeof(TaskLogic)].As<TaskLogic>().CancelTask;
        mainLogics[typeof(TaskLogic)].As<TaskLogic>().SetTaskbarActive(false);

        InitState();

        await base.OnInit();
    }

    protected override async void ButtonOnClick(string btnName)
    {
        switch (btnName)
        {
            case "btnTask":
                await UIManager.Instance.CreateViewAsync<TaskView, TaskModel, TaskController>(E_UILayer.Mid);
                break;
            case "btnBattleTest":
                await BattleManager.Instance.StartBattle();
                break;
        }
    }

    private void Active()
    {
        UIManager.Instance.SetViewActive<MainController>(true);
    }

    private void InActive()
    {
        UIManager.Instance.SetViewActive<MainController>(false);
    }

    /// <summary>
    /// 初始化界面状态
    /// </summary>
    private void InitState()
    {
        foreach (var item in mainLogics.Values)
        {
            item.Init();
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        EventCenter.Instance.RemoveEventListener<List<IInteractable>>(E_EventType.E_OnInteract, mainLogics[typeof(InteractLogic)].As<InteractLogic>().CreateInteract);
    }
}
