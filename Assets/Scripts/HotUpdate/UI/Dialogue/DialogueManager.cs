using System;
using System.Collections;
using System.Text;
using Core.DI;
using Core.Exceptions;
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
        [Inject] private IUIService _uiService;
        [Inject] private IEventCenter _eventCenter;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IMainDataProvider _mainDataManger;
        
        private readonly IPoolManager _poolManager;
        // 对话上下文
        private readonly DialogueContext _dialogueContext;
        // 分支处理器缓存
        private readonly Lazy<BranchHandlerCollecor> _branchHandlerCollecor;

        private DialogueController DialogueController => (DialogueController)_uiService.GetPanel(EUIPanelId.DialoguePanel);
        
        public event Action OnDialogueStart;

        public event Action OnDialogueEnd;

        public event Action<DialogueInfo> OnSingleDialogueStart;

        public event Action OnSingleDialogueEnd;

        public event Action<BranchInfo> OnSelectDialogueBranch;
        
        public bool IsDialogueActive => _dialogueContext.IsDialogueActive;

        private DialogueManager(IPoolManager poolManager)
        {
            _branchHandlerCollecor = new Lazy<BranchHandlerCollecor>(() => DIContainer.Create<BranchHandlerCollecor>());
            _dialogueContext = poolManager.GetData<DialogueContext>();
            _poolManager = poolManager;
        }
        
        public async void StartDialogue(int startDialogueId)
        {
            try
            {
                // 已有对话在进行时，不重复启动
                if (_dialogueContext.IsDialogueActive)
                {
                    return;
                }
                
                _dialogueContext.EnableTypewriter = _mainDataManger.GameSettings.GetInt(ESettingType.TypeWriter) != 0;    // 0为false，1为true，自定义规则
                // 标记对话为进行中
                _dialogueContext.IsDialogueActive = true;
                SetNextDialogue(startDialogueId);
                // 触发对话开始事件
                OnDialogueStart?.Invoke();
                // 加载并创建对话UI，获取控制器
                await _uiService.OpenAsync(EUIPanelId.DialoguePanel, E_UILayer.Mid, hideMain: true);
                DialogueController.SetDialogueBoxActive(true);
                ShowCurrentDialogue();
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Dialogue, e);
            }
        }

        public void SetNextDialogue(int nextDialogueId)
        {
            DialogueInfo dialogueInfo;
            if (nextDialogueId == -1)
            {
                dialogueInfo = new DialogueInfo { f_id = -1 };
            }
            else
            {
                // 从配置表中获取对话信息
                dialogueInfo = _binaryDataManager.GetConfig<DialogueInfoContainer>(EConfigLoadType.Excel).dataDic[nextDialogueId];
            }
            
            // 记录当前对话信息
            _dialogueContext.CurrentDialogueInfo = dialogueInfo;
        }

        /// <summary>
        /// 显示当前设置的ID的对话内容
        /// </summary>
        public void ShowCurrentDialogue()
        {
            // 对话ID为-1时，结束整个对话流程
            if (_dialogueContext.CurrentDialogueInfo.f_id == -1)
            {
                EndDialogue();
                return;
            }
            
            // 从配置表中获取说话者（NPC）信息
            _dialogueContext.NpcInfo = _binaryDataManager.GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[_dialogueContext.CurrentDialogueInfo.f_speakerId];
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
                throw ExceptionHelper.Throw("Dialogue Controller Not Set");
            
            // 无对话进行时，直接返回
            if (!IsDialogueActive)
                return;

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
                    SetNextDialogue(_dialogueContext.CurrentDialogueInfo.f_nextId);
                    ShowCurrentDialogue();
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
            if (_branchHandlerCollecor.Value.TryGetHandler(branchData.BranchType, out var branchHandler))
            {
                branchHandler.Execute(branchData);
            }

            OnSelectDialogueBranch?.Invoke(branchData.BranchInfo);
        }

        /// <summary>
        /// 结束整个对话流程
        /// </summary>
        public async void EndDialogue()
        {
            // 销毁对话界面
            await _uiService.CloseAsync(DialogueController.panelId, true, true);
            // 触发全局对话事件
            _eventCenter.TriggerEvent(new DialogueEvent(_dialogueContext.NpcInfo.f_id));
            // 触发对话结束事件
            OnDialogueEnd?.Invoke();
            _poolManager.PushData(_dialogueContext);
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