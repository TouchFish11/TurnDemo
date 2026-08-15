using System;
using HotUpdate.Game.Dialogue.Datas;
using HotUpdate.Game.Dialogue.Sources;

namespace HotUpdate.Game.Dialogue
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
        /// <param name="branchData"></param>
        void OnSelectOpt(BranchData branchData);

        /// <summary>
        /// 结束对话
        /// </summary>
        void EndDialogue();

        /// <summary>
        /// 添加新分支来源，重复添加会失败
        /// </summary>
        /// <param name="branchDataSource"></param>
        /// <returns>是否添加成功</returns>
        bool AddBranchSource(IBranchDataSource branchDataSource);
        
        /// <summary>
        /// 移除指定的分支来源
        /// </summary>
        /// <param name="branchDataSource"></param>
        /// <returns>是否移除成功</returns>
        bool RemoveBranchSource(IBranchDataSource branchDataSource);

        /// <summary>
        /// 显示指定ID的对话内容
        /// </summary>
        /// <param name="startDialogueId">要显示的对话ID</param>
        void ShowCurrentDialogue(int startDialogueId);

        event Action<BranchInfo> OnSelectDialogueBranch;
    }
}
