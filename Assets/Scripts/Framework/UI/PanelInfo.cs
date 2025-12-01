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
        private TView _view;
        // UI数据
        private TModel _model;
        // UI控制器
        private TController _controller;

        public PanelInfo(TView view, TModel model, TController uIController)
        {
            _view = view;
            _model = model;
            _controller = uIController;
        }

        /// <summary>
        /// 面板对象
        /// </summary>
        public TView View => _view;

        public TModel Model => _model;

        public TController UIController => _controller;
    }
}
