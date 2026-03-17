using System.Threading.Tasks;
using Core.UI;
using Core.UI.MVC;
using HotUpdate.Common;
using HotUpdate.Core.UI.Helper;
using HotUpdate.Core.UI.MVC;
using HotUpdate.Task.UI;

namespace HotUpdate.Task.Core
{
    public class TaskUiHelper : ITaskUiHelper
    {
        private readonly IUIManager _uiManager;

        public TaskUiHelper(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public async Task<ITaskController> CreateTaskController()
        {
            return await _uiManager.CreateViewAsync<TaskView, TaskModel, TaskController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.TaskView);
        }

        public async Task<IuiController> GetUiController()
        {
            return await CreateTaskController();
        }
    }
}
