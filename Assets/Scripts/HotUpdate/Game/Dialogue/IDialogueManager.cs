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
        /// 是否有对话正在进行中
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
        /// 对话分支选择回调
        /// </summary>
        event Action<BranchInfo> OnSelectDialogueBranch;

        /// <summary>
        /// 启动对话，内部会创建对话界面，隐藏主界面；
        /// 内部会自动调用<see cref="SetNextDialogue"/>，调用该方法时无需设置
        /// </summary>
        /// <param name="startDialogueId"></param>
        void StartDialogue(int startDialogueId);

        /// <summary>
        /// 推进对话
        /// </summary>
        /// <exception cref="Exception">未先调用<see cref="StartDialogue"/>时抛出</exception>
        void NextDialogue();

        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="branchData"></param>
        void OnSelectOpt(BranchData branchData);

        /// <summary>
        /// 结束对话，会隐藏对话界面显示主界面
        /// </summary>
        void EndDialogue();

        /// <summary>
        /// 添加新分支来源，不允许重复添加
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
        /// 显示当前设置的ID的对话内容，在调用前先执行<see cref="SetNextDialogue"/>设置对话
        /// </summary>
        void ShowCurrentDialogue();

        /// <summary>
        /// 设置下一条对话
        /// </summary>
        /// <param name="nextDialogueId"></param>
        void SetNextDialogue(int nextDialogueId);
    }
}
