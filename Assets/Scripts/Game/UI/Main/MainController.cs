using Framework;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
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
    // 交互逻辑
    private readonly InteractLogic interactLogic;
    // 任务逻辑
    private readonly TaskLogic taskLogic;

    public MainController(MainView view, MainModel model) : base(view, model)
    {
        interactLogic = new InteractLogic(this, model, view);
        taskLogic = new TaskLogic(this, model, view);
    }

    protected override async Task OnInit()
    {
        EventCenter.Instance.AddEventListener<List<IInteractable>>(E_EventType.E_OnInteract, interactLogic.CreateInteract);
        // 对话事件监听
        DialogueManager.Instance.OnDialogueStart += interactLogic.DeactivateInteract;
        DialogueManager.Instance.OnDialogueEnd += interactLogic.ActiveInteract;
        // 任务事件监听
        TaskManager.Instance.OnUpdateTask += taskLogic.UpdateTask;
        TaskManager.Instance.OnCancelTask += taskLogic.CancelTask;
        taskLogic.CancelTask();

        await base.OnInit();
    }

    protected override async void ButtonOnClick(string btnName)
    {
        switch (btnName)
        {
            case "btnTask":
                await UIManager.Instance.CreateViewAsync<TaskView, TaskModel, TaskController>(E_UILayer.Mid);
                break;
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        EventCenter.Instance.RemoveEventListener<List<IInteractable>>(E_EventType.E_OnInteract, interactLogic.CreateInteract);
        DialogueManager.Instance.OnDialogueStart -= interactLogic.DeactivateInteract;
        DialogueManager.Instance.OnDialogueEnd -= interactLogic.ActiveInteract;
    }
}
