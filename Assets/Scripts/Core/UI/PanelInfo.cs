using Core.AssetBundles.Management;
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
        
        public PoolObject PoolObject { get; private set; }
        
        public IuiController Controller { get; }
        public IuiView View { get; }
        
        public PanelInfo(int id, PoolObject<T> poolObject, IuiView view, IuiController uIController)
        {
            _id = id;
            PoolObject = poolObject;
            View = view;
            Controller = uIController;
        }
    }
}
