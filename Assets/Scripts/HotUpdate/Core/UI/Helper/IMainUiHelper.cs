using System.Threading.Tasks;
using HotUpdate.Core.UI.MVC;

namespace HotUpdate.Core.UI.Helper
{
    public interface IMainUiHelper : IUiHelper
    {
        Task<IMainController> CreateMainController();
        
        Task<IBackController> CreateBackController();
        
        Task<IBattleLoadingController> CreateBattleLoadingController();
    }
}
