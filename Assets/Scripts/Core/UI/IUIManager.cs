using System.Threading.Tasks;
using Core.UI.MVC;
using UnityEngine;

namespace Core.UI
{
    /// <summary>
    /// UI管理器接口
    /// </summary>
    public interface IUIManager
    {
        Canvas Canvas { get; }
        
        Camera UICamera { get; }
        
        Transform GetLayer(E_UILayer layer);
        
        TController GetController<TController>() where TController : IuiController;
        
        Task InitUIManagerAsync();

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
            where TView : UIBehaviourBase, IuiView where TModel : IuiModel, new() where TController : class, IuiController, new();

        /// <summary>
        /// 销毁界面
        /// </summary>
        void DestroyView(IuiController controller);

        /// <summary>
        /// 设置界面活动状态
        /// 只能设置第一个查找到的实例，多实例无法准确获取
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="isActive"></param>
        void SetViewActive(IuiController controller, bool isActive);
    }
}
