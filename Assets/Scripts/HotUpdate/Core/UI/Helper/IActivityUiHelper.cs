using HotUpdate.Core.MVC;

namespace HotUpdate.Core.UI.Helper
{
    public interface IActivityUiHelper : IUiHelper
    {
        System.Threading.Tasks.Task<IActivityController> CreateActivityController();
    }
}
