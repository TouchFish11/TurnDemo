using Core.UI.ViewController;

namespace Core.UI
{
    /// <summary>
    /// 界面信息
    /// </summary>
    public class PanelInfo<T> : IPanelInfo where T : UIView
    {
        // 界面ID
        private int _id;
        
        public IuiController Controller { get; }
        
        public UIView View { get; }

        public PanelInfo(int id, T view, IuiController uIController)
        {
            _id = id;
            View = view;
            Controller = uIController;
        }
    }
}
