using System.Threading.Tasks;
using Core.UI.MVC;

namespace HotUpdate.Core.UI.Helper
{
    public interface IUiHelper
    {
        Task<IuiController> GetUiController();
    }
}
