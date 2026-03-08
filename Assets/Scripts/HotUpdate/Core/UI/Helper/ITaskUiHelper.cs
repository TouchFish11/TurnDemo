using System.Threading.Tasks;
using HotUpdate.Core.MVC;

namespace HotUpdate.Core.UI.Helper
{
    public interface ITaskUiHelper : IUiHelper
    {
        public Task<ITaskController> CreateTaskController();
    }
}
