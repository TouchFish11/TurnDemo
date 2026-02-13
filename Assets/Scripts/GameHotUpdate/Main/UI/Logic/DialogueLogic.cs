using Core.Service;
using Core.UI;
using Game.Dialogue;

namespace GameHotUpdate.Main.UI.Logic
{
    public class DialogueLogic : MainLogic
    {
        public DialogueLogic(MainController mainController, MainModel mainModel, MainView mainView) : base(mainController, mainModel, mainView)
        {
            
        }
        
        public override void Init()
        {
            // 注册对话系统回调：对话开始时隐藏主界面
            ServiceLocator.Get<IDialogueManager>().OnDialogueStart += InActive;
            // 注册对话系统回调：对话结束时显示主界面
            ServiceLocator.Get<IDialogueManager>().OnDialogueEnd += Active;
        }
        
        /// <summary>
        /// 激活主界面
        /// 作用：设置主界面为显示状态
        /// </summary>
        private void Active()
        {
            ServiceLocator.Get<IUIManager>().SetViewActive(mainController,true);
        }

        /// <summary>
        /// 隐藏主界面
        /// 作用：设置主界面为隐藏状态
        /// </summary>
        private void InActive()
        {
            ServiceLocator.Get<IUIManager>().SetViewActive(mainController,false);
        }

        public override void Dispose()
        {
            ServiceLocator.Get<IDialogueManager>().OnDialogueStart -= InActive;
            ServiceLocator.Get<IDialogueManager>().OnDialogueEnd -= Active;
            base.Dispose();
        }
    }
}
