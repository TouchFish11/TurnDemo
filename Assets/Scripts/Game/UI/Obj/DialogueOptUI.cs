using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 对话分支选项UI
/// </summary>
public class DialogueOptUI : UIBehaviour
{
    protected UIComponentBinder uIComponentBinder;
    private BranchInfo branchInfo;

    /// <summary>
    /// 选择选项事件
    /// </summary>
    public event Action<int> OnSelectOpt;

    protected override void Awake()
    {
        uIComponentBinder = new UIComponentBinder(this);
        uIComponentBinder.Bind();

        uIComponentBinder.OnButtonClick += OnOptionClick;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="branchInfo"></param>
    public void Init(BranchInfo branchInfo)
    {
        this.branchInfo = branchInfo;
        uIComponentBinder.GetControl<TextMeshProUGUI>("txtOptText").text = branchInfo.f_optText;
    }

    private void OnOptionClick(string optName)
    {
        // 选择该分支选项
        OnSelectOpt?.Invoke(branchInfo.f_dialogueId);
    }
}
