using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 面板信息类
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TController"></typeparam>
    public class PanelInfo<TView, TModel, TController> : BasePanelInfo 
        where TView : UIView where TModel : UIModel, new() where TController : UIController<TView, TModel>
    {
        public PanelInfo(TView view, TModel model, TController uIController)
        {
            View = view;
            Model = model;
            Controller = uIController;
        }

        public override UIView View { get; protected set; }

        public override UIModel Model { get; protected set; }

        public override IUIController Controller { get; protected set; }
    }
}
