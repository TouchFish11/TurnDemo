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

    public MainController(MainView view, MainModel model) : base(view, model)
    {
        interactLogic = new InteractLogic(this, model, view);
    }

    protected override async Task OnInit()
    {
        EventCenter.Instance.AddEventListener<List<IInteractable>>(E_EventType.E_OnInteract, interactLogic.CreateInteract);
        DialogueManager.Instance.OnDialogueStart += _model.DeactivateInteract;
        DialogueManager.Instance.OnDialogueEnd += _model.ActiveInteract;

        await base.OnInit();
    }

    protected override async void ButtonOnClick(string btnName)
    {
        switch (btnName)
        {
            case "btnTask":
                await UIManager.Instance.ShowViewAsync<TaskView, TaskModel, TaskController>(E_UILayer.Mid);
                break;
        }
    }

    private void UpdateTask()
    {
        _model.IsActiveTaskbar = true;
    }


    public override void Destroy()
    {
        base.Destroy();
        EventCenter.Instance.RemoveEventListener<List<IInteractable>>(E_EventType.E_OnInteract, interactLogic.CreateInteract);
        DialogueManager.Instance.OnDialogueStart -= _model.DeactivateInteract;
        DialogueManager.Instance.OnDialogueEnd -= _model.ActiveInteract;
    }
}
