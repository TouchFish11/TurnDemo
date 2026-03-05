using System;
using Core.Log;
using Core.Service;
using Core.UI;
using HotUpdate.Dialogue;

namespace HotUpdate.Main.UI.Logic
{
    /// <summary>
    /// 主界面对话逻辑
    /// </summary>
    public class DialogueLogic : MainLogic
    {
        private readonly IDialogueManager _dialogueManager = ServiceLocator.Get<IDialogueManager>();
        private readonly IUIManager _uiManager = ServiceLocator.Get<IUIManager>();
        
        public override void Init(MainController mainController, MainModel mainModel, MainView mainView)
        {
            base.Init(mainController, mainModel, mainView);
            // 注册对话系统回调：对话开始时隐藏主界面
            _dialogueManager.OnDialogueStart += InActive;
            // 注册对话系统回调：对话结束时显示主界面
            _dialogueManager.OnDialogueEnd += Active;
        }
        
        /// <summary>
        /// 激活主界面
        /// 作用：设置主界面为显示状态
        /// </summary>
        private async void Active()
        {
            try
            {
                await _uiManager.SetViewActive(mainController,true);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(DialogueLogic)}.{nameof(Active)}：{e.Message}，{e.StackTrace}");
            }
        }

        /// <summary>
        /// 隐藏主界面
        /// 作用：设置主界面为隐藏状态
        /// </summary>
        private async void InActive()
        {
            try
            {
                await _uiManager.SetViewActive(mainController,false);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(DialogueLogic)}.{nameof(InActive)}：{e.Message}，{e.StackTrace}");
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
