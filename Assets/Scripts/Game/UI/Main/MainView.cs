using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Ö÷½çÃæ
/// </summary>
public class MainView : UIView
{
    private GameObject taskPart;

    protected override void Awake()
    {
        base.Awake();

        taskPart = this.transform.Find(nameof(taskPart)).gameObject;
    }

    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "interactUIs":
                List<InteractUI> interactUIs = value as List<InteractUI>;
                foreach (InteractUI interactUI in interactUIs)
                {
                    interactUI.transform.SetParent(uIComponentBinder.GetControl<ScrollRect>("svInteract").content, false);
                }
                break;
            case "isActiveTaskbar":
                taskPart.SetActive((bool)value);
                break;
        }
    }
}
