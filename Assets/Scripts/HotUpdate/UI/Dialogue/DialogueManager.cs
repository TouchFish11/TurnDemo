using System;
using System.Collections;
using System.Text;
using Core.DI;
using Core.GlobalEvent;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Serialize.Binary;
using Core.UI;
using HotUpdate.Base.Data;
using HotUpdate.Base.Settings;
using HotUpdate.Base.UI;
using HotUpdate.Common.Events;
using HotUpdate.Game.Dialogue;
using HotUpdate.Game.Dialogue.Datas;
using HotUpdate.Game.Dialogue.Sources;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Dialogue
{
    /// <summary>
    /// 对话管理器
    /// 负责对话的启动、逐字显示、分支选择、下一步对话、结束对话等核心逻辑
    /// </summary>
    public class DialogueManager : IDialogueManager, IDisposable
    {
        [Inject] private IPoolManager _poolManager;
        [Inject] private IUIService _uiService;
        [Inject] private IEventCenter _eventCenter;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IMainDataProvider _mainDataManger;
        
        // 对话上下文
        private DialogueContext _dialogueContext;
        // 分支处理器缓存
        private readonly BranchHandlerCollecor _branchHandlerCollecor;

        private DialogueController DialogueController => (DialogueController)_uiService.GetPanel(EUIPanelId.DialoguePanel);
        
        // 对话开始事件（整段对话流程启动时触发）
        public event Action OnDialogueStart;
        // 对话结束事件（整段对话流程结束时触发）
        public event Action OnDialogueEnd;
        // 单条对话开始事件（单条对话播放前触发）
        public event Action<DialogueInfo> OnSingleDialogueStart;
        // 单条对话结束事件（单条对话播放完成后触发）
        public event Action OnSingleDialogueEnd;
        // 对话分支选择回调
        public event Action<BranchInfo> OnSelectDialogueBranch;

        /// <summary>
        /// 是否有对话正在进行中
        /// </summary>
        public bool IsDialogueActive => _dialogueContext.IsDialogueActive;

        private DialogueManager(BranchHandlerCollecor collecor)
        {
            _branchHandlerCollecor = collecor;
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
                if (_dialogueContext.IsDialogueActive)
                {
                    return;
                }

                _dialogueContext = _poolManager.GetData<DialogueContext>();
                _dialogueContext.EnableTypewriter = (int)_mainDataManger.GameSettings[ESettingType.TypeWriter] != 0;    // 0为false，1为true，自定义规则
                // 加载并创建对话UI，获取控制器
                await _uiService.OpenAsync(EUIPanelId.DialoguePanel, E_UILayer.Mid);
                // 标记对话为进行中
                _dialogueContext.IsDialogueActive = true;
                // 触发对话开始事件
                OnDialogueStart?.Invoke();
                // 显示起始ID对应的对话内容
                ShowCurrentDialogue(startDialogueId);
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Dialogue, e);
            }
        }

        /// <summary>
        /// 显示指定ID的对话内容
        /// </summary>
        /// <param name="startDialogueId">要显示的对话ID</param>
        public void ShowCurrentDialogue(int startDialogueId)
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
            _dialogueContext.CurrentDialogueInfo = dialogueInfo;
            // 从配置表中获取说话者（NPC）信息
            _dialogueContext.NpcInfo = _binaryDataManager.GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[dialogueInfo.f_speakerId];
            if (_dialogueContext.EnableTypewriter)
            {
                // 启用打字机效果：初始化状态+启动协程
                _dialogueContext.DialogueOver = false;
                _dialogueContext.TypewriterCor = _monoAdapter.StartCoroutine(ApplyTypewriter());
                OnSingleDialogueStart?.Invoke(_dialogueContext.CurrentDialogueInfo);
            }
            else
            {
                // 禁用打字机：直接显示完整文本+显示分支选项
                _dialogueContext.DialogueOver = true;
                DialogueController.ShowDialogueText(_dialogueContext.NpcInfo.f_speakerName, _dialogueContext.CurrentDialogueInfo.f_dialgueText);
                ShowBranchOpt();
            }
        }

        /// <summary>
        /// 执行打字机效果（逐字显示对话文本）
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator ApplyTypewriter()
        {
            var text = _dialogueContext.CurrentDialogueInfo.f_dialgueText;
            var sb = new StringBuilder(text.Length); // 拼接逐字文本
            foreach (var t in text)
            {
                sb.Append(t);
                // 逐帧更新对话文本显示
                DialogueController.ShowDialogueText(_dialogueContext.NpcInfo.f_speakerName, sb.ToString());
                // 等待字符间隔时间
                yield return new WaitForSeconds(_dialogueContext.TypewriterInterval);
            }
            // 标记单条对话播放完成
            _dialogueContext.DialogueOver = true;
            OnSingleDialogueEnd?.Invoke();
            // 显示分支选项（如果有）
            ShowBranchOpt();
        }

        /// <summary>
        /// 切换到下一条对话
        /// </summary>
        public void NextDialogue()
        {
            if (DialogueController == null)
            {
                throw new Exception("Dialogue Controller Not Set");
            }
            
            // 无对话进行时，直接返回
            if (!IsDialogueActive)
            {
                return;
            }

            // 打字机未播放完成时：停止协程+直接显示完整文本
            if (!_dialogueContext.DialogueOver && _dialogueContext.TypewriterCor != null)
            {
                _monoAdapter.StopCoroutine(_dialogueContext.TypewriterCor);
                DialogueController.ShowDialogueText(_dialogueContext.NpcInfo.f_speakerName, _dialogueContext.CurrentDialogueInfo.f_dialgueText);
                _dialogueContext.DialogueOver = true;
                OnSingleDialogueEnd?.Invoke();
                ShowBranchOpt();
            }
            // 打字机已完成且无分支时：切换到下一条对话
            else
            {
                if (!_dialogueContext.CurrentDialogueInfo.f_hasBranch)
                {
                    ShowCurrentDialogue(_dialogueContext.CurrentDialogueInfo.f_nextId);
                }
            }
        }

        public bool AddBranchSource(IBranchDataSource branchDataSource)
        {
            return _dialogueContext.CurrentBranchSources.TryAdd(branchDataSource.GetType(), branchDataSource);
        }

        public bool RemoveBranchSource(IBranchDataSource branchDataSource)
        {
            return _dialogueContext.CurrentBranchSources.Remove(branchDataSource.GetType());
        }
        
        /// <summary>
        /// 显示对话分支选项
        /// </summary>
        private void ShowBranchOpt()
        {
            _dialogueContext.BranchDatas.Clear();
            foreach (var branchSource in _dialogueContext.CurrentBranchSources.Values)
            {
                _dialogueContext.BranchDatas.AddRange(branchSource.GetBranchDatas(_dialogueContext));
            }
            
            // 给UI控制器设置分支选项，显示到界面
            DialogueController.SetBranchOpt(_dialogueContext.BranchDatas.ToArray());
        }

        /// <summary>
        /// 玩家选择分支选项后的回调
        /// </summary>
        /// <param name="branchData"></param>
        public void OnSelectOpt(BranchData branchData)
        {
            if (_branchHandlerCollecor.TryGetHandler(branchData.BranchType, out var branchHandler))
            {
                branchHandler.Execute(branchData);
            }

            OnSelectDialogueBranch?.Invoke(branchData.BranchInfo);
        }

        /// <summary>
        /// 结束整个对话流程
        /// </summary>
        public void EndDialogue()
        {
            _poolManager.PushData(_dialogueContext);
            // 销毁对话UI
            _uiService.CloseAsync(DialogueController.panelId, true);
            // 触发全局对话事件
            _eventCenter.TriggerEvent(new DialogueEvent(_dialogueContext.NpcInfo.f_id));
            // 触发对话结束事件
            OnDialogueEnd?.Invoke();
        }

        public void Dispose()
        {
            _uiService = null;
            _eventCenter = null;
            _binaryDataManager = null;
            _monoAdapter = null;
        }
    }
}