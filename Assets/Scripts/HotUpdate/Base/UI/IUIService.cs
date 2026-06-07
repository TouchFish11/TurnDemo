using System.Threading.Tasks;
using Core.UI;
using Core.UI.ViewController;
using UnityEngine;

namespace HotUpdate.Base.UI
{
    public interface IUIService
    {
        Task<IuiController> OpenAsync(EUIPanelId panelId, E_UILayer layer, Vector2 pos = default, Quaternion quaternion = default);
        
        Task CloseAsync(int panelId, bool isDestroy);
        
        IuiController GetPanel(EUIPanelId panelId);
        
        /// <summary>
        /// 显示隐藏的界面，若界面被销毁则不处理
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        Task ShowAsync(int panelId);
    }
}
