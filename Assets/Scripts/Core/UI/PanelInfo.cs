using Core.UI.ViewController;

namespace Core.UI
{
    /// <summary>
    /// 界面信息
    /// </summary>
    internal class PanelInfo
    {
        /// <summary>
        /// 界面控制器接口
        /// </summary>
        public IuiController Controller { get; protected set; }
        
        /// <summary>
        /// 界面视图接口
        /// </summary>
        public UIView View { get; protected set; }
    }
    
    /// <summary>
    /// 界面信息
    /// </summary>
    internal class PanelInfo<T> : PanelInfo where T : UIView
    {
        // 界面ID
        private int _id;

        public PanelInfo(int id, T view, IuiController uIController)
        {
            _id = id;
            View = view;
            Controller = uIController;
        }
    }
}
