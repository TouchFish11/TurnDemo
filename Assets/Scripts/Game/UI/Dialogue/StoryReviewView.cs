using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 剧情回顾界面
/// </summary>
public class StoryReviewView : UIBehaviour
{
    private UIComponentBinder uIComponentBinder;
    private ScrollRect svReview;
    // 历史对话信息列表
    private readonly List<DialogueInfo> historicalDialogueInfos = new List<DialogueInfo>();
    // 对话回顾UI列表
    private readonly List<DialogueReviewUI> dialogueReviewUIs = new List<DialogueReviewUI>();

    /// <summary>
    /// 子界面关闭事件
    /// </summary>
    public event Action OnSubViewClosed;

    protected override void Awake()
    {
        uIComponentBinder = new UIComponentBinder(this);
        uIComponentBinder.OnButtonClick += OnButtonClick;

        svReview = uIComponentBinder.GetControl<ScrollRect>(nameof(svReview));
    }

    protected override void OnEnable()
    {
        Show();
    }

    private void OnButtonClick(string btnName)
    {
        switch (btnName)
        {
            case "btnClose":
                OnSubViewClosed?.Invoke();
                break;
        }
    }

    /// <summary>
    /// 显示
    /// </summary>
    private async void Show()
    {
        foreach (DialogueInfo dialogueInfo in historicalDialogueInfos)
        {
            GameObject reviewObj = await PoolManager.Instance.GetAssetBundleObjAsync(E_AssetBundleType.UI, ResConfigCollection.DialogueReviewUI);
            DialogueReviewUI dialogueReviewUI = reviewObj.GetComponent<DialogueReviewUI>();
            dialogueReviewUI.Init(dialogueInfo.f_speakerName, dialogueInfo.f_dialgueText);
            dialogueReviewUI.transform.SetParent(svReview.content, false);
            dialogueReviewUIs.Add(dialogueReviewUI);
        }
    }

    /// <summary>
    /// 缓存历史对话信息
    /// </summary>
    /// <param name="dialogueReviewUI"></param>
    public void CacheDialogueInfo(DialogueInfo dialogueInfo)
    {
        historicalDialogueInfos.Add(dialogueInfo);
    }

    /// <summary>
    /// 清除回顾文本UI
    /// </summary>
    private void ClearReviewUI()
    {
        foreach (DialogueReviewUI dialogueReviewUI in dialogueReviewUIs)
        {
            PoolManager.Instance.PushObj(dialogueReviewUI.gameObject);
        }
        dialogueReviewUIs.Clear();
    }

    protected override void OnDisable()
    {
        ClearReviewUI();
    }
}
