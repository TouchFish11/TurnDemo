using System.Threading.Tasks;
using Core.UI.MVC;
using UnityEngine;

namespace Core.UI
{
    /// <summary>
    /// UI�������ӿ�
    /// </summary>
    public interface IUIManager
    {
        Canvas Canvas { get; }
        Camera UICamera { get; }
        
        Transform GetLayer(E_UILayer layer);
        
        TController GetController<TController>() where TController : IuiController;
        
        Task InitUIManagerAsync();
        
        void SetViewActive<TController>(bool isActive) where TController : IuiController;

        /// <summary>
        /// 异步显示界面
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TController"></typeparam>
        /// <param name="layer"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        Task<TController> CreateViewAsync<TView, TModel, TController>(E_UILayer layer, string panelName)
            where TView : BaseUIBehaviour, IuiView where TModel : IuiModel, new() where TController : class, IuiController, new();

        /// <summary>
        /// 销毁界面
        /// </summary>
        void DestroyView(IuiController controller);
    }
}
