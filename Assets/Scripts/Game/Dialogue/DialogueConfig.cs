using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单流程对话配置
/// </summary>
[CreateAssetMenu]
public class DialogueConfig : ScriptableObject
{
    // 线性对话文本
    public SingleDialogue[] dialogues;
    // 是否有分支选项
    public bool hasBranch;
    // 分支选项
    public DialogueBranch[] branches;
}
