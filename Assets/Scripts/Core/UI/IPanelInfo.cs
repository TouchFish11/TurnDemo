using Core.UI.MVC;

namespace Core.UI
{
    public interface IPanelInfo
    {
        IuiController UiController { get; }
        
        IuiView UiView { get; }
    }
    
    /// <summary>
    /// 界面信息接口
    /// </summary>
    public interface IPanelInfo<out TView, out TModel, out TController> : IPanelInfo where TView : IuiView where TModel : IuiModel where TController : IuiController
    {
        TView View { get; }
        
        TModel Model { get; }

        TController Controller { get; }
    }
}
