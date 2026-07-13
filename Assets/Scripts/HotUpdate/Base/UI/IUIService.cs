using System.Threading.Tasks;
using Core.UI;
using Core.UI.ViewController;
using UnityEngine;

namespace HotUpdate.Base.UI
{
    public interface IUIService
    {
        /// <summary>
        /// 异步打开界面，当界面已经被创建时会重复创建
        /// </summary>
        /// <param name="panelId"></param>
        /// <param name="layer"></param>
        /// <param name="pos"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        Task<IuiController> OpenAsync(EUIPanelId panelId, E_UILayer layer, Vector2 pos = default, Quaternion quaternion = default);
        
        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="panelId"></param>
        /// <param name="isDestroy"></param>
        /// <returns></returns>
        Task CloseAsync(int panelId, bool isDestroy);
        
        /// <summary>
        /// 获取已经打开的界面对象
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        IuiController GetPanel(EUIPanelId panelId);
        
        /// <summary>
        /// 显示隐藏的界面，若界面被销毁则不处理
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        Task ShowAsync(int panelId);
    }
}
