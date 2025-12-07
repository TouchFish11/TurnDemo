using Framework;
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
    // 对话选项UI列表
    private readonly List<DialogueOptUI> dialogueOptUIs = new List<DialogueOptUI>();
    // 对话框是否显示
    private bool isActiveBox;
    // 剧情回顾界面音乐
    private StoryReviewView storyReviewView;
    // 回顾界面是否显示
    private bool isActiveReview;
    // 对话提示效果
    private string txtTip;

    /// <summary>
    /// 设置剧情回顾界面
    /// </summary>
    /// <param name="storyReviewView"></param>
    public void SetStoryReviewView(StoryReviewView storyReviewView)
    {
        this.storyReviewView = storyReviewView;
    }

    /// <summary>
    /// 设置分支选项
    /// </summary>
    /// <param name="optUIs"></param>
    public void SetBranchOpt(IEnumerable<DialogueOptUI> optUIs)
    {
        foreach (DialogueOptUI opt in dialogueOptUIs)
        {
            PoolManager.Instance.PushObj(opt.gameObject);
        }
        dialogueOptUIs.Clear();

        if (optUIs == null)
        {
            return;
        }

        dialogueOptUIs.AddRange(optUIs);
        TriggerDataChanged(nameof(dialogueOptUIs), dialogueOptUIs);
    }

    /// <summary>
    /// 清理分支选项
    /// </summary>
    private void ClearBranchOpt()
    {
        foreach (DialogueOptUI opt in dialogueOptUIs)
        {
            PoolManager.Instance.PushObj(opt.gameObject);
        }
        dialogueOptUIs.Clear();
    }

    /// <summary>
    /// 添加回顾UI文本
    /// </summary>
    /// <param name="dialogueReview"></param>
    public void CacheDialogueInfo(DialogueInfo dialogueInfo)
    {
        storyReviewView.CacheDialogueInfo(dialogueInfo);
    }

    /// <summary>
    /// 设置对话提示效果
    /// </summary>
    /// <param name="text"></param>
    public void SetTip(string text)
    {
        txtTip = text;
        TriggerDataChanged(nameof(txtTip), text);
    }

    public bool IsActiveReview
    {
        get => isActiveReview;
        set
        {
            isActiveReview = value;
            TriggerDataChanged(nameof(isActiveReview), value);
        }
    }

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

    public bool IsActiveBox
    {
        get => isActiveBox;
        set
        {
            isActiveBox = value;
            TriggerDataChanged(nameof(isActiveBox), value);
        }
    }

    public override void ClearData()
    {
        ClearBranchOpt();
    }
}
