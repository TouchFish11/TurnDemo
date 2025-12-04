using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话界面数据
/// </summary>
public class DialogueModel : UIModel
{
    // 说话者
    private string speakName;
    // 对话文本内容
    private string dialogueText;
    // 是否隐藏对话界面
    private bool isHideView;
    // 是否正在播放对话
    private bool isPlaying;
    // 是否自动播放
    private bool isAutoPlay;

    public string SpeakName
    {
        get => speakName;
        set
        {
            speakName = value;
            TriggerDataChanged(nameof(speakName), value);
        }
    }

    public string DialogueText
    {
        get => dialogueText;
        set
        {
            dialogueText = value;
            TriggerDataChanged(nameof(dialogueText), value);
        }
    }

    public bool IsHideView
    {
        get => isHideView;
        set
        {
            isHideView = value;
            TriggerDataChanged(nameof(isHideView), value);
        }
    }

    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            isPlaying = value;
            TriggerDataChanged(nameof(isPlaying), value);
        }
    }

    public bool IsAutoPlay
    {
        get => isAutoPlay;
        set
        {
            isAutoPlay = value;
            TriggerDataChanged(nameof(isAutoPlay), value);
        }
    }
}
