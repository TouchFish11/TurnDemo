using System;
using Core.DI;
using Core.Log;
using Core.UI;
using HotUpdate.Base.Manager;
using HotUpdate.Game.Main.UI.Logic;

namespace HotUpdate.UI.Main.Logic
{
    /// <summary>
    /// 主界面对话逻辑
    /// </summary>
    public class DialogueLogic : MainLogic
    {
        [Inject] private IDialogueManager _dialogueManager;
        [Inject] private IUIManager _uiManager;
        
        protected override void OnInit()
        {
            // 注册对话系统回调：对话开始时隐藏主界面
            _dialogueManager.OnDialogueStart += InActive;
            // 注册对话系统回调：对话结束时显示主界面
            _dialogueManager.OnDialogueEnd += Active;
        }

        /// <summary>
        /// 激活主界面
        /// 设置主界面为显示状态
        /// </summary>
        private async void Active()
        {
            try
            {
                await _uiManager.SetViewActive(mainController.panelId,true);
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(DialogueLogic)}.{nameof(Active)}：激活主界面错误，{e.Message}");
            }
        }

        /// <summary>
        /// 隐藏主界面
        /// 设置主界面为隐藏状态
        /// </summary>
        private async void InActive()
        {
            try
            {
                await _uiManager.SetViewActive(mainController.panelId,false);
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(DialogueLogic)}.{nameof(InActive)}：隐藏主界面错误，{e.Message}");
            }
        }

        public override void ResetData()
        {
            _dialogueManager.OnDialogueStart -= InActive;
            _dialogueManager.OnDialogueEnd -= Active;
            base.ResetData();
        }
    }
}
