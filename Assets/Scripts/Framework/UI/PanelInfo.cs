using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 面板信息类
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public class PanelInfo<TView, TModel, TController> : BasePanelInfo 
        where TView : UIView where TModel : UIModel, new() where TController : UIController<TView, TModel>
    {
        // 视图对象
        //private readonly TView view;
        //// UI数据
        //private readonly TModel model;
        //// UI控制器
        //private readonly TController _controller;

        public PanelInfo(TView view, TModel model, TController uIController)
        {
            View = view;
            Model = model;
            Controller = uIController;
        }

        /// <summary>
        /// 面板对象
        /// </summary>
        //public TView View => view;

        public override UIView View { get; protected set; }

        //public TModel Model => model;

        public override UIModel Model { get; protected set; }

        //public TController UIController => _controller;

        public override IUIController Controller { get; protected set; }
    }
}
