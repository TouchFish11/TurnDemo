using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ¶Ô»°»Ø¹ËUI
/// </summary>
public class DialogueReviewUI : UIBehaviour
{
    private TextMeshProUGUI txtDialogueText;

    protected override void Awake()
    {
        txtDialogueText = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(string speakerName, string dialogueText)
    {
        txtDialogueText.text = $"{speakerName}£º{dialogueText}";
    }
}
