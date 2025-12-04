using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    // 是否启用打字机效果（全局配置，可让玩家选择）
    private bool enableTypewriter;

    /// <summary>
    /// 打字机打字间隔
    /// </summary>
    private const float TypewriterInterval = 0.2f;

    public DialogueController(DialogueView view, DialogueModel model) : base(view, model)
    {

    }

    protected override void OnInit()
    {
        enableTypewriter = true;
    }

    protected override void ButtonOnClick(string btnName)
    {
        switch (btnName)
        {
            case "btnContinue":

                break;
            case "btnHide":

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

    /// <summary>
    /// 显示对话文本
    /// </summary>
    /// <param name="dialogueInfo"></param>
    public void ShowDialogueText(string speakerName ,string dialogueText)
    {
        // 显示说话者
        _model.SpeakName = speakerName;
        // 显示对话内容
        _model.DialogueText = dialogueText;
    }
}
