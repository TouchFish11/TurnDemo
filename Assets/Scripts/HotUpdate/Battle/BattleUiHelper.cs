using System.Threading.Tasks;
using Core.UI;
using Core.UI.MVC;
using HotUpdate.Battle.UI.Base;
using HotUpdate.Common;
using HotUpdate.Core.MVC;
using HotUpdate.Core.UI;
using HotUpdate.Core.UI.Helper;

namespace HotUpdate.Battle
{
    public class BattleUiHelper : IBattleUiHelper
    {
        private readonly IUIManager _uiManager;

        public BattleUiHelper(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public async Task<IuiController> GetUiController()
        {
            return await CreateBattleController();
        }

        public async Task<IBattleController> CreateBattleController()
        {
            return await _uiManager.CreateViewAsync<BattleView, BattleModel, BattleController>(AbKeyCollection.Ui, E_UILayer.Bot, ResKeyCollection.BattleView);
        }
    }
}
