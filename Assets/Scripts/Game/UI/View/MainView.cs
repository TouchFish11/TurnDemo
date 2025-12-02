using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ö÷½çÃæ
/// </summary>
public class MainView : UIView
{
    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "svInteract":
                List<InteractUI> interactUIs = value as List<InteractUI>;
                foreach (InteractUI interactUI in interactUIs)
                {
                    interactUI.transform.SetParent(uIComponentBinder.GetControl<ScrollRect>(key).content, false);
                }
                break;
        }
    }
}
