using System.Threading.Tasks;
using Core.DI;
using Core.UI;
using HotUpdate.Game.Dialogue;
using HotUpdate.Game.Dialogue.Sources;

namespace HotUpdate.UI.Main.Logic
{
    /// <summary>
    /// 主界面对话逻辑
    /// </summary>
    public class DialogueLogic : MainLogic
    {
        [Inject] private IDialogueManager _dialogueManager;
        [Inject] private IUIManager _uiManager;
        
        protected override Task OnInit()
        {
            // 注册对话系统回调
            _dialogueManager.OnDialogueStart += InActive;
            // 注册对话系统回调
            _dialogueManager.OnDialogueEnd += Active;
            
            // Test: 添加对话分支
            _dialogueManager.AddBranchSource(DIContainer.Create<DialogueConfigBranchDataSource>());
            return Task.CompletedTask;
        }
        
        private async void Active()
        {

        }
        
        private async void InActive()
        {

        }

        protected override void OnResetData()
        {
            _dialogueManager.OnDialogueStart -= InActive;
            _dialogueManager.OnDialogueEnd -= Active;
        }
    }
}
