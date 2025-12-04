using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话分支
/// </summary>
[Serializable]
public class DialogueBranch
{
    // 选项文本
    public string optionText;
    // 选该选项后触发的下一段对话
    public DialogueConfig nextDialogue;
    //// 选该选项后触发的事件（如“AcceptQuest”）
    //public string callbackEvent;
}
