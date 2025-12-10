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
    public MainController(MainView view, MainModel model) : base(view, model)
    {

    }

    protected override async Task OnInit()
    {
        EventCenter.Instance.AddEventListener<List<IInteractable>>(E_EventType.E_OnInteract, CreateInteract);
        DialogueManager.Instance.OnDialogueStart += DeactivateInteract;
        DialogueManager.Instance.OnDialogueEnd += ActiveInteract;
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

    /// <summary>
    /// 创建交互UI
    /// </summary>
    /// <param name="interactables"></param>
    private async void CreateInteract(List<IInteractable> interactables)
    {
        List<InteractUI> interactUIs = new List<InteractUI>(interactables.Count);
        foreach (IInteractable interactable in interactables)
        {
            GameObject interactInstance = await PoolManager.Instance.GetAssetBundleObjAsync(E_AssetBundleType.UI, "InteractUI");
            InteractUI interactUI = interactInstance.GetComponent<InteractUI>();
            // 初始化文本
            interactUI.Init(interactable.NpcConfig.npcName);
            interactUIs.Add(interactUI);
        }
        // 设置交互UI
        _model.SetInteracts(interactUIs);
    }

    /// <summary>
    /// 激活交互UI
    /// </summary>
    private void ActiveInteract()
    {
        _model.ActiveInteract();
    }

    /// <summary>
    /// 失活交互UI
    /// </summary>
    private void DeactivateInteract()
    {
        _model.DeactivateInteract();
    }


    public override void Destroy()
    {
        base.Destroy();
        EventCenter.Instance.RemoveEventListener<List<IInteractable>>(E_EventType.E_OnInteract, CreateInteract);
        DialogueManager.Instance.OnDialogueStart -= DeactivateInteract;
        DialogueManager.Instance.OnDialogueEnd -= ActiveInteract;
    }
}
