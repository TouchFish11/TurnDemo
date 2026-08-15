using Core.DI;
using HotUpdate.Game.Dialogue.Datas;

namespace HotUpdate.Game.Dialogue.Handler
{
    /// <summary>
    /// 对话分支处理器，执行对话逻辑
    /// </summary>
    public class DialogueBranchHandler : IBranchHandler
    {
        [Inject] private IDialogueManager _dialogueManager;

        public EBranchType BranchType => EBranchType.Dialogue;

        public void Execute(BranchData branchData)
        {
            // 显示选中分支对应的对话
            _dialogueManager.ShowCurrentDialogue(branchData.BranchInfo.f_dialogueId);
        }
    }
}
