using Core.Service;
using Core.UI;
using GameHotUpdate.Dialogue;

namespace GameHotUpdate.Main.UI.Logic
{
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
        private void Active()
        {
            _uiManager.SetViewActive(mainController,true);
        }

        /// <summary>
        /// 隐藏主界面
        /// 作用：设置主界面为隐藏状态
        /// </summary>
        private void InActive()
        {
            _uiManager.SetViewActive(mainController,false);
        }

        public override void ResetData()
        {
            _dialogueManager.OnDialogueStart -= InActive;
            _dialogueManager.OnDialogueEnd -= Active;
            base.ResetData();
        }
    }
}
