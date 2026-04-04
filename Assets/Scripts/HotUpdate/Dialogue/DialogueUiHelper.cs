using System.Threading.Tasks;
using Core.UI;
using Core.UI.MVC;
using HotUpdate.Common;
using HotUpdate.Core.UI.Helper;
using HotUpdate.Core.UI.MVC;
using HotUpdate.Dialogue.UI;

namespace HotUpdate.Dialogue
{
    public class DialogueUiHelper : IDialogueUiHelper
    {
        private readonly IUIManager _uiManager;

        public DialogueUiHelper(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public async Task<IuiController> GetUiController()
        {
            return await CreateDialogueController();
        }

        public async Task<IDialogueController> CreateDialogueController()
        {
            return await _uiManager.CreateViewAsync<DialogueView, DialogueModel, DialogueController>(AbKeyCollection.Ui, E_UILayer.Bot, ResKeyCollection.DialogueView);
        }
    }
}
