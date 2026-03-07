using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Core.Global;
using Core.GlobalEvent;
using Core.Loader.Object;
using Core.Log;
using Core.Mono;
using Core.Serialize.Binary;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.Utility;
using HotUpdate.Common;
using HotUpdate.Core.Dialogue;
using HotUpdate.Dialogue.UI;
using UnityEngine;

namespace HotUpdate.Dialogue
{
    /// <summary>
    /// 对话管理器（单例）
    /// 负责对话的启动、逐字显示、分支选择、下一步对话、结束对话等核心逻辑
    /// </summary>
    public class DialogueManager : SingletonBase<DialogueManager>, IDialogueManager
    {
        public override int Priority => -1;
        private readonly IUIManager _uiManager = ServiceLocator.Get<IUIManager>();
        private readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        private readonly IEventCenter _eventCenter = ServiceLocator.Get<IEventCenter>();
        private readonly IBinaryDataManager _binaryDataManager = ServiceLocator.Get<IBinaryDataManager>();
        private readonly IMonoAdapter _monoAdapter = ServiceLocator.Get<IMonoAdapter>();
        
        // 是否启用打字机效果（仅在全局设置开启时生效）
        private bool enableTypewriter;
        // 当前单条对话是否播放完成（打字机/直接显示）
        private bool dialogueOver;
        // 打字机效果的协程引用
        private Coroutine typewriterCor;
        // 当前正在显示的对话信息
        private DialogueInfo currentDialogueInfo;
        // 对话UI控制器（用于操作对话界面显示）
        private DialogueController dialogueController;
        // 当前对话的NPC信息（说话者）
        private NpcInfo npcInfo;

        /// <summary>
        /// 打字机效果字符间隔（秒）
        /// </summary>
        private const float TypewriterInterval = 0.05f;

        // 对话开始事件（整段对话流程启动时触发）
        public event Action OnDialogueStart;
        // 对话结束事件（整段对话流程结束时触发）
        public event Action OnDialogueEnd;
        // 对话分支选择事件（玩家选择分支选项时触发）
        public event Action OnBranchSelected;
        // 单条对话开始事件（单条对话播放前触发）
        public event Action<DialogueInfo> OnSingleDialogueStart;
        // 单条对话结束事件（单条对话播放完成后触发）
        public event Action OnSingleDialogueEnd;

        // 是否有对话正在进行中（对外只读）
        public bool IsDialogueActive { get; private set; } 
        
        private DialogueManager()
        {

        }

        public override Task InitAsync()
        {
            enableTypewriter = true;
            // 监听全局设置中打字机效果的开关变化
            GameSettingManager.Instance.OnEnableTypewriterChanged += OnEnableTypewriterChanged;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 全局打字机开关变更回调
        /// </summary>
        /// <param name="value">新的开关状态</param>
        private void OnEnableTypewriterChanged(bool value)
        {
            enableTypewriter = value;
        }

        /// <summary>
        /// 启动对话流程
        /// </summary>
        /// <param name="startDialogueId">起始对话ID</param>
        public async void StartDialogue(int startDialogueId)
        {
            try
            {
                // 已有对话在进行时，不重复启动
                if (IsDialogueActive)
                {
                    return;
                }

                // 加载并创建对话UI，获取控制器
                dialogueController = await _uiManager.CreateViewAsync<DialogueView, DialogueModel, DialogueController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.DialogueView);
                // 标记对话为进行中
                IsDialogueActive = true;
                // 触发对话开始事件
                OnDialogueStart?.Invoke();
                // 显示起始ID对应的对话内容
                ShowCurrentDialogue(startDialogueId);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(DialogueManager)}.{nameof(StartDialogue)}：{e.Message}，{e.StackTrace}");
            }
        }

        /// <summary>
        /// 显示指定ID的对话内容
        /// </summary>
        /// <param name="startDialogueId">要显示的对话ID</param>
        private void ShowCurrentDialogue(int startDialogueId)
        {
            // 对话ID为-1时，结束整个对话流程
            if (startDialogueId == -1)
            {
                EndDialogue();
                return;
            }

            // 从配置表中获取对话信息
            var dialogueInfo = _binaryDataManager.GetConfig<DialogueInfoContainer>(EConfigLoadType.Excel).dataDic[startDialogueId];
            // 记录当前对话信息
            currentDialogueInfo = dialogueInfo;
            // 从配置表中获取说话者（NPC）信息
            npcInfo = _binaryDataManager.GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[dialogueInfo.f_speakerId];

            if (enableTypewriter)
            {
                // 启用打字机效果：初始化状态+启动协程
                dialogueOver = false;
                typewriterCor = _monoAdapter.StartCoroutine(ApplyTypewriter());
                OnSingleDialogueStart?.Invoke(currentDialogueInfo);
            }
            else
            {
                // 禁用打字机：直接显示完整文本+显示分支选项
                dialogueOver = true;
                dialogueController.ShowDialogueText(npcInfo.f_speakerName, currentDialogueInfo.f_dialgueText);
                ShowBranchOpt();
            }
        }

        /// <summary>
        /// 执行打字机效果（逐字显示对话文本）
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator ApplyTypewriter()
        {
            var text = currentDialogueInfo.f_dialgueText;
            var sb = new StringBuilder(text.Length); // 拼接逐字文本
            foreach (var t in text)
            {
                sb.Append(t);
                // 逐帧更新对话文本显示
                dialogueController.ShowDialogueText(npcInfo.f_speakerName, sb.ToString());
                // 等待字符间隔时间
                yield return new WaitForSeconds(TypewriterInterval);
            }
            // 标记单条对话播放完成
            dialogueOver = true;
            OnSingleDialogueEnd?.Invoke();
            // 显示分支选项（如果有）
            ShowBranchOpt();
        }

        /// <summary>
        /// 切换到下一条对话
        /// </summary>
        public void NextDialogue()
        {
            // 无对话进行时，直接返回
            if (!IsDialogueActive)
            {
                return;
            }

            // 打字机未播放完成时：停止协程+直接显示完整文本
            if (!dialogueOver && typewriterCor != null)
            {
                _monoAdapter.StopCoroutine(typewriterCor);
                dialogueController.ShowDialogueText(npcInfo.f_speakerName, currentDialogueInfo.f_dialgueText);
                dialogueOver = true;
                OnSingleDialogueEnd?.Invoke();
                ShowBranchOpt();
            }
            // 打字机已完成且无分支时：切换到下一条对话
            else
            {
                if (!currentDialogueInfo.f_hasBranch)
                {
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
                // 解析分支ID数组（配置表中以特定格式存储）
                var branchIds = TextUtility.SplitToIntArr(currentDialogueInfo.f_branchIds, 2);
                var branchInfos = new BranchInfo[branchIds.Length];

                // 遍历分支ID，从配置表加载分支信息
                for (var i = 0; i < branchIds.Length; i++)
                {
                    branchInfos[i] = _binaryDataManager.GetConfig<BranchInfoContainer>(EConfigLoadType.Excel).dataDic[branchIds[i]];
                }
                // 给UI控制器设置分支选项，显示到界面
                dialogueController.SetBranchOpt(branchInfos);
            }
        }

        /// <summary>
        /// 玩家选择分支选项后的回调
        /// </summary>
        /// <param name="dialogueId">分支对应的下一条对话ID</param>
        public void OnSelectOpt(int dialogueId)
        {
            // 显示选中分支对应的对话
            ShowCurrentDialogue(dialogueId);
            // 触发分支选择事件
            OnBranchSelected?.Invoke();
        }

        /// <summary>
        /// 结束整个对话流程
        /// </summary>
        public void EndDialogue()
        {
            // 标记对话为未进行状态
            IsDialogueActive = false;
            // 销毁对话UI
            _uiManager.DestroyView(AbKeyCollection.Ui, dialogueController);
            // 触发全局对话事件
            _eventCenter.TriggerEvent(new DialogueEvent { NpcId = npcInfo.f_id });
            // 触发对话结束事件
            OnDialogueEnd?.Invoke();
            // 清理对话选项UI对象池
            _prefabLoader.RealseAsset(AbKeyCollection.Ui, ResKeyCollection.DialogueOptUI);
            // 清理对话回顾UI对象池
            _prefabLoader.RealseAsset(AbKeyCollection.Ui, ResKeyCollection.DialogueReviewUI);
        }
    }
}