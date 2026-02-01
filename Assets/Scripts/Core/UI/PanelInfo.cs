using Core.UI.MVC;

namespace Core.UI
{
    /// <summary>
    /// �����Ϣ��
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TController"></typeparam>
    public class PanelInfo<TView, TModel, TController> : IPanelInfo<TView, TModel, TController>
        where TView : IuiView where TModel : IuiModel where TController : IuiController
    {
        public TView View { get; }
        
        public TModel Model { get; }
        
        public TController Controller { get; }
        
        public IuiController UiController => Controller;
        
        public IuiView UiView => View;

        public PanelInfo(TView view, TModel model, TController uIController)
        {
            View = view;
            Model = model;
            Controller = uIController;
        }
    }
}
