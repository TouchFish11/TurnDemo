using System.Threading.Tasks;
using HotUpdate.Core.UI.MVC;

namespace HotUpdate.Core.UI.Helper
{
    public interface ITaskUiHelper : IUiHelper
    {
        public Task<ITaskController> CreateTaskController();
    }
}
