using Core.AssetBundles.Management;
using Core.UI.ViewController;

namespace Core.UI
{
    public interface IPanelInfo
    {
        /// <summary>
        /// 界面缓存对象
        /// </summary>
        PoolObject PoolObject { get; }
        
        /// <summary>
        /// 界面控制器接口
        /// </summary>
        IuiController Controller { get; }
        
        /// <summary>
        /// 界面视图接口
        /// </summary>
        IuiView View { get; }
    }
}
