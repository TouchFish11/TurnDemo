using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 对话界面控制器工厂
/// </summary>
public class DialogueControllerFactory : UIControllerFactory<DialogueView, DialogueModel, DialogueController>
{
    public override DialogueController CreateController(DialogueView view, DialogueModel model)
    {
        return new DialogueController(view, model);
    }

    public override DialogueModel CreateModel()
    {
        return new DialogueModel();
    }
}

/// <summary>
/// 对话界面控制器
/// </summary>
public class DialogueController : UIController<DialogueView, DialogueModel>
{
    private static readonly WaitForSeconds _waitForSeconds0_25 = new WaitForSeconds(0.25f);

    /// <summary>
    /// 对话提示
    /// </summary>
    private const string DialogueTip = "...";
    /// <summary>
    /// 默认提示文本
    /// </summary>
    private const string DefaultTip = "V";

    // 提示效果文本协程
    private Coroutine dialogueTipCor;

    public DialogueController(DialogueView view, DialogueModel model) : base(view, model)
    {

    }

    protected override async Task OnInit()
    {
        ServiceLocator.Instance.Get<IDialogueManager>().OnSingleDialogueStart += OnSingleDialogueStart;
        ServiceLocator.Instance.Get<IDialogueManager>() .OnSingleDialogueEnd += OnSingleDialogueEnd;

        StoryReviewView storyReviewView = view.GetComponentInChildren<StoryReviewView>();
        storyReviewView.OnSubViewClosed += OnSubViewClosed;
        model.SetStoryReviewView(view.GetComponentInChildren<StoryReviewView>());
        model.IsActiveReview = false;

        await base.OnInit();
    }

    /// <summary>
    /// 子界面关闭
    /// </summary>
    private void OnSubViewClosed()
    {
        model.IsActiveReview = false;
    }

    protected override void ButtonOnClick(string btnName)
    {
        switch (btnName)
        {
            case "btnContinue":
                // 对话框显示时，才能推进对话
                if (model.IsActiveBox)
                {
                    ServiceLocator.Instance.Get<IDialogueManager>().NextDialogue();
                }
                else
                {
                    // 否则先显示对话框
                    model.IsActiveBox = true;
                }
                break;
            case "btnHide":
                model.IsActiveBox = false;
                break;
            case "btnReview":
                model.IsActiveReview = true;
                break;
        }
    }

    protected override void ToggleValueChanged(string toggleName, bool isOn)
    {
        switch (toggleName)
        {
            case "togAuto":

                break;
        }
    }

    private void OnSingleDialogueStart(DialogueInfo dialogueInfo)
    {
        // 缓存历史对话
        model.CacheDialogueInfo(dialogueInfo);
        // 提示效果协程
        dialogueTipCor = MonoManager.Instance.StartCoroutine(DialogueTip_Cor());
    }

    private void OnSingleDialogueEnd()
    {
        MonoManager.Instance.StopCoroutine(dialogueTipCor);
        // 重置文本
        model.SetTip(DefaultTip);
    }

    // 对话提示效果协程
    public IEnumerator DialogueTip_Cor()
    {
        int length = DialogueTip.Length;
        while (true)
        {
            for (int i = 0; i < length; i++)
            {
                model.SetTip(DialogueTip.Substring(0, i + 1));
                yield return _waitForSeconds0_25;
            }
        }
    }

    /// <summary>
    /// 显示对话文本
    /// </summary>
    /// <param name="dialogueInfo"></param>
    public void ShowDialogueText(string speakerName ,string dialogueText)
    {
        // 设置分支选项
        model.SetBranchOpt(null);
        // 显示说话者
        model.SpeakName = speakerName;
        // 显示对话内容
        model.DialogueText = dialogueText;
    }

    /// <summary>
    /// 设置分支选项
    /// </summary>
    /// <param name="branchInfos"></param>
    public async void SetBranchOpt(BranchInfo[] branchInfos)
    {
        List<DialogueOptUI> dialogueOpts = new List<DialogueOptUI>(branchInfos.Length);

        foreach (BranchInfo branchInfo in branchInfos)
        {
            DialogueOptUI optUI = await ObjectBuilder.GetObject<DialogueOptUI>(E_AssetBundleType.UI, ResKeyCollection.DialogueOptUI, null);
            // 初始化
            optUI.Init(branchInfo);
            optUI.OnSelectOpt += ServiceLocator.Instance.Get<IDialogueManager>().OnSelectOpt;
            dialogueOpts.Add(optUI);
        }
        // 设置分支选项
        model.SetBranchOpt(dialogueOpts);
    }
}
