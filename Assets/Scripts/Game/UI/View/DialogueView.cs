using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对话界面
/// </summary>
public class DialogueView : UIView
{
    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "speakName":
                uIComponentBinder.GetControl<TextMeshProUGUI>("txtSpeakerName").text = value as string;
                break;
            case "dialogueText":
                uIComponentBinder.GetControl<TextMeshProUGUI>("txtDialogue").text = value as string;
                break;
            case "dialogueOptUIs":
                Transform transform = uIComponentBinder.GetControl<VerticalLayoutGroup>("DialogueOptBox").transform;
                List<DialogueOptUI> dialogueOptUIs = value as List<DialogueOptUI>;
                foreach (DialogueOptUI opt in dialogueOptUIs)
                {
                    opt.transform.SetParent(transform, false);
                }
                break;
        }
    }
}
