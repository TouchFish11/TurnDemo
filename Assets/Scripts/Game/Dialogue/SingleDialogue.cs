using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单句对话
/// </summary>
[Serializable]
public class SingleDialogue
{
    // 说话人
    public string speakerName;
    // 对话文本（多行显示）
    [TextArea(2, 5)] public string dialogueText;
    // 说话人头像
    public Sprite speakerIcon;
    // 语音（可选）
    public AudioClip voiceClip;
}
