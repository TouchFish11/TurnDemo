using System.Threading.Tasks;
using Core.UI;
using Core.UI.MVC;
using HotUpdate.Common;
using HotUpdate.Core.UI.Helper;
using HotUpdate.Core.UI.MVC;
using HotUpdate.Main.Back;
using HotUpdate.Main.Loading.Battle;
using HotUpdate.Main.UI;

namespace HotUpdate.Main
{
    public class MainUiHelper : IMainUiHelper
    {
        private readonly IUIManager _uiManager;

        public MainUiHelper(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public async Task<IMainController> CreateMainController()
        {
           return await _uiManager.CreateViewAsync<MainView, MainModel, MainController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.MainView);
        }

        public async Task<IBackController> CreateBackController()
        {
            return await _uiManager.CreateViewAsync<BackView, BackModel, BackController>(AbKeyCollection.Ui, E_UILayer.Bot, ResKeyCollection.BackView);
        }

        public async Task<IBattleLoadingController> CreateBattleLoadingController()
        {
            return await _uiManager.CreateViewAsync<BattleLoadingView, BattleLoadingModel, BattleLoadingController>(AbKeyCollection.Ui, E_UILayer.Bot, ResKeyCollection.BattleLoadingView);
        }

        Task<IuiController> IUiHelper.GetUiController()
        {
            return null;
        }
    }
}
