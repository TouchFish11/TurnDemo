using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 对话管理器
/// </summary>
public class DialogueManager : SingletonBase<DialogueManager>, IDialogueManager
{
    // 是否启用打字机效果（全局配置，可让玩家选择）
    private bool enableTypewriter;
    // 当前单句对话是否完成
    private bool dialogueOver;
    // 打字机效果协程
    private Coroutine typewriterCor;
    // 当前对话信息
    private DialogueInfo currentDialogueInfo;
    // 对话界面控制器
    private DialogueController dialogueController;
    // 当前对话的NpcId
    private NpcInfo npcInfo;

    /// <summary>
    /// 打字机打字间隔
    /// </summary>
    private const float TypewriterInterval = 0.05f;

    public event Action OnDialogueStart;

    public event Action OnDialogueEnd;

    public event Action OnBranchSelected;

    public event Action<DialogueInfo> OnSingleDialogueStart;

    public event Action OnSingleDialogueEnd;

    public bool IsDialogueActive { get; private set; } 

    private DialogueManager()
    {
        enableTypewriter = true;
        GameSettingManager.Instance.OnEnableTypewriterChanged += OnEnableTypewriterChanged;
    }

    private void OnEnableTypewriterChanged(bool value)
    {
        enableTypewriter = value;
    }

    public async void StartDialogue(int startDialogueId)
    {
        if (IsDialogueActive)
        {
            return;
        }

        // 获取对话界面控制器
        dialogueController = await UIManager.Instance.CreateViewAsync<DialogueView, DialogueModel, DialogueController>(E_UILayer.Mid);
        // 对话中
        IsDialogueActive = true;
        // 触发“对话开始”事件
        OnDialogueStart?.Invoke();
        // 显示当前对话
        ShowCurrentDialogue(startDialogueId);
    }

    /// <summary>
    /// 显示当前对话
    /// </summary>
    private void ShowCurrentDialogue(int startDialogueId)
    {
        if (startDialogueId == -1)
        {
            EndDialogue();
            return;
        }

        // 获取该ID的对话信息
        DialogueInfo dialogueInfo = BinaryDataManager.Instance.GetConfig<DialogueInfoContainer>(E_ConfigLoadType.Excel).dataDic[startDialogueId];
        // 记录当前对话信息
        currentDialogueInfo = dialogueInfo;
        // 记录当前对话的Npc信息
        npcInfo = BinaryDataManager.Instance.GetConfig<NpcInfoContainer>(E_ConfigLoadType.Excel).dataDic[dialogueInfo.f_speakerId];

        if (enableTypewriter)
        {
            dialogueOver = false;
            // 逐字显示
            typewriterCor = MonoManager.Instance.StartCoroutine(ApplyTypewriter());
            OnSingleDialogueStart?.Invoke(currentDialogueInfo);
        }
        else
        {
            dialogueOver = true;
            // 直接显示对话文本
            dialogueController.ShowDialogueText(npcInfo.f_speakerName, currentDialogueInfo.f_dialgueText);
            // 显示对话分支（若有）
            ShowBranchOpt();
        }
    }

    /// <summary>
    /// 应用打字机效果
    /// </summary>
    /// <param name="dialogueInfo"></param>
    /// <returns></returns>
    private IEnumerator ApplyTypewriter()
    {
        string text = currentDialogueInfo.f_dialgueText;
        StringBuilder sb = new StringBuilder(text.Length);
        for (int i = 0; i < text.Length; i++)
        {
            sb.Append(text[i]);
            dialogueController.ShowDialogueText(npcInfo.f_speakerName, sb.ToString());
            yield return new WaitForSeconds(TypewriterInterval);
        }
        dialogueOver = true;
        OnSingleDialogueEnd?.Invoke();
        ShowBranchOpt();
    }

    public void NextDialogue()
    {
        if (!IsDialogueActive)
        {
            return;
        }

        // 若启用打字机效果，且未完成时，则停止效果直接显示完整文本
        if (!dialogueOver && typewriterCor != null)
        {
            MonoManager.Instance.StopCoroutine(typewriterCor);
            dialogueController.ShowDialogueText(npcInfo.f_speakerName, currentDialogueInfo.f_dialgueText);
            dialogueOver = true;
            OnSingleDialogueEnd?.Invoke();
            ShowBranchOpt();
        }
        // 推进对话
        else
        {
            if (!currentDialogueInfo.f_hasBranch)
            {
                // 显示下一ID的对话
                ShowCurrentDialogue(currentDialogueInfo.f_nextId);
            }
        }
    }

    /// <summary>
    /// 显示对话分支选项
    /// </summary>
    private void ShowBranchOpt()
    {
        if (currentDialogueInfo.f_hasBranch)
        {
            int[] branchIds = TextUtility.SplitToIntArr(currentDialogueInfo.f_branchIds, 2);
            BranchInfo[] branchInfos = new BranchInfo[branchIds.Length];

            for (int i = 0; i < branchIds.Length; i++)
            {
                branchInfos[i] = BinaryDataManager.Instance.GetConfig<BranchInfoContainer>(E_ConfigLoadType.Excel).dataDic[branchIds[i]];
            }
            dialogueController.SetBranchOpt(branchInfos);
        }
    }

    public void OnSelectOpt(int dialogueId)
    {
        ShowCurrentDialogue(dialogueId);
        OnBranchSelected?.Invoke();
    }

    public void EndDialogue()
    {
        // 重置标志
        IsDialogueActive = false;
        // 隐藏对话UI
        UIManager.Instance.DestroyView();
        // 分发对话结束事件
        EventCenter.Instance.TriggerEvent(E_EventType.E_OnDialogue, new DialogueEvent() { npcId = npcInfo.f_id });
        // 触发“对话结束”事件
        OnDialogueEnd?.Invoke();
        // 清理对话选项UI缓存
        PoolManager.Instance.ClearType<DialogueOptUI>();
        // 清理对话回顾UI缓存
        PoolManager.Instance.ClearType<DialogueReviewUI>();
    }
}
