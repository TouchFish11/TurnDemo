using System.Threading.Tasks;
using HotUpdate.Core.MVC;

namespace HotUpdate.Core.UI.Helper
{
    public interface IBattleUiHelper : IUiHelper
    {
        Task<IBattleController> CreateBattleController();
    }
}
