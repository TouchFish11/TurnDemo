using HotUpdate.Core.UI.MVC;

namespace HotUpdate.Core.UI.Helper
{
    public interface IActivityUiHelper : IUiHelper
    {
        System.Threading.Tasks.Task<IActivityController> CreateActivityController();
    }
}
