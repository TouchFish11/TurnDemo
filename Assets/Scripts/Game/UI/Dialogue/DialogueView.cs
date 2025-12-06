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
    private TextMeshProUGUI txtSpeakerName;
    private TextMeshProUGUI txtTip;
    private TextMeshProUGUI txtDialogue;
    private Text txtAuto;
    // 对话框
    private Transform dialogueBox;
    // 选项框
    private Transform dialogueOptBox;
    // 剧情回顾子界面
    private Transform storyReviewSubView;
    // 剧情回顾子界面容器
    private Transform storyReviewContent;

    protected override void Awake()
    {
        base.Awake();

        txtSpeakerName = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtSpeakerName));
        txtTip = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTip));
        txtDialogue = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtDialogue));
        txtAuto = uIComponentBinder.GetControl<Text>(nameof(txtAuto));

        dialogueOptBox = uIComponentBinder.GetControl<VerticalLayoutGroup>("DialogueOptBox").transform;
        dialogueBox = this.transform.Find(nameof(dialogueBox));

        storyReviewSubView.gameObject.SetActive(false);
        storyReviewContent = uIComponentBinder.GetControl<ScrollRect>("svReview").content;
    }

    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "speakName":
                txtSpeakerName.text = value as string;
                break;
            case "dialogueText":
                txtDialogue.text = value as string;
                break;
            case "dialogueOptUIs":
                List<DialogueOptUI> dialogueOptUIs = value as List<DialogueOptUI>;
                foreach (DialogueOptUI opt in dialogueOptUIs)
                {
                    opt.transform.SetParent(dialogueOptBox, false);
                }
                break;
            case "isActiveBox":
                dialogueBox.gameObject.SetActive((bool)value);
                break;
            case "dialogueReview":
                DialogueReviewUI dialogueReviewUI = value as DialogueReviewUI;
                dialogueReviewUI.transform.SetParent(storyReviewContent, false);
                break; 
        }
    }
}
