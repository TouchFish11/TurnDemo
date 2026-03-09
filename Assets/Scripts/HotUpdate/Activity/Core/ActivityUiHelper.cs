using System.Threading.Tasks;
using Core.UI;
using Core.UI.MVC;
using HotUpdate.Activity.UI.Base;
using HotUpdate.Common;
using HotUpdate.Core.MVC;
using HotUpdate.Core.UI.Helper;

namespace HotUpdate.Activity.Core
{
    public class ActivityUiHelper : IActivityUiHelper
    {
        private readonly IUIManager _uiManager;
        
        public ActivityUiHelper(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public async Task<IActivityController> CreateActivityController()
        {
            return await _uiManager.CreateViewAsync<ActivityView, ActivityModel, ActivityController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.ActivityView);
        }
        
        public async Task<IuiController> GetUiController()
        {
            return await CreateActivityController();
        }
    }
}
