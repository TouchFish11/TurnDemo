using System;

namespace HotUpdate.Base.Manager
{
    /// <summary>
    /// 对话管理器接口
    /// </summary>
    public interface IDialogueManager
    {
        /// <summary>
        /// 是否正在显示对话
        /// </summary>
        bool IsDialogueActive { get; }

        /// <summary>
        /// 对话开始
        /// </summary>
        event Action OnDialogueStart;

        /// <summary>
        /// 对话结束
        /// </summary>
        event Action OnDialogueEnd;

        /// <summary>
        /// 分支选择
        /// </summary>
        event Action OnBranchSelected;

        /// <summary>
        /// 单句对话开始事件
        /// </summary>
        event Action<DialogueInfo> OnSingleDialogueStart;

        /// <summary>
        /// 单句对话结束事件
        /// </summary>
        event Action OnSingleDialogueEnd;

        /// <summary>
        /// 启动对话
        /// </summary>
        /// <param name="startDialogueId"></param>
        void StartDialogue(int startDialogueId);

        /// <summary>
        /// 推进对话
        /// </summary>
        void NextDialogue();

        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="dialogueId"></param>
        void OnSelectOpt(int dialogueId);

        /// <summary>
        /// 结束对话
        /// </summary>
        void EndDialogue();
    }
}
