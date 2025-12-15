using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主界面交互逻辑类
/// </summary>
public class InteractLogic : MainLogic
{
    public InteractLogic(MainController mainController, MainModel mainModel, MainView mainView) : base(mainController, mainModel, mainView)
    {

    }

    /// <summary>
    /// 创建交互UI
    /// </summary>
    /// <param name="interactables"></param>
    public async void CreateInteract(List<IInteractable> interactables)
    {
        List<InteractUI> interactUIs = new List<InteractUI>(interactables.Count);
        foreach (IInteractable interactable in interactables)
        {
            InteractUI interactUI = await ObjectBuilder.GetOrCreateInstance<InteractUI>(E_AssetBundleType.UI, ResConfigCollection.InteractUI, null);
            // 初始化文本
            interactUI.Init(interactable.NpcConfig.npcName);
            interactUIs.Add(interactUI);
        }
        // 设置交互UI
        mainModel.SetInteracts(interactUIs);
    }

    /// <summary>
    /// 激活交互UI
    /// </summary>
    public void ActiveInteract()
    {
        mainModel.ActiveInteract();
    }

    /// <summary>
    /// 失活交互UI
    /// </summary>
    public void DeactivateInteract()
    {
        mainModel.DeactivateInteract();
    }
}
