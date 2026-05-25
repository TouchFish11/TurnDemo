using System.Threading.Tasks;
using Core.UI;
using Core.UI.ViewController;
using UnityEngine;

namespace HotUpdate.Base.UI
{
    public interface IUIService
    {
        Task<IuiController> OpenAsync(EUIPanelId panelId, E_UILayer layer, Vector2 pos = default, Quaternion quaternion = default);
        
        Task CloseAsync(int panelId);
        IuiController GetPanel(EUIPanelId panelId);
    }
}
