using System.Threading.Tasks;
using HotUpdate.Core.MVC;

namespace HotUpdate.Core.UI.Helper
{
    public interface IDialogueUiHelper : IUiHelper
    {
        Task<IDialogueController> CreateDialogueController();
    }
}
