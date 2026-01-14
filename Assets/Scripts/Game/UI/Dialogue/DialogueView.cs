using Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对话界面
/// </summary>
public class DialogueView : UIView
{
    [Inject] private ScrollRect svReview;
    [Inject] private TextMeshProUGUI txtSpeakerName;
    [Inject] private TextMeshProUGUI txtTip;
    [Inject] private TextMeshProUGUI txtDialogue;
    [Inject] private Text txtAuto;
    // 对话框
    [Inject] private RectTransform dialogueBox;
    // 选项框
    [Inject] private RectTransform dialogueOptBox;
    // 剧情回顾子界面
    [Inject] private RectTransform storyReviewSubView;
    // 剧情回顾子界面容器
    private RectTransform storyReviewContent;

    protected override void Awake()
    {
        base.Awake();
        storyReviewContent = svReview.content;
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
            case "isActiveReview":
                storyReviewSubView.gameObject.SetActive((bool)value);
                break;
            case "txtTip":
                txtTip.text = value.ToString();
                break;
        }
    }
}
