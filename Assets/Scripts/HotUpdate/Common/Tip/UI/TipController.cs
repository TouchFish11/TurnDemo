using Core.UI.MVC;

namespace HotUpdate.Common.Tip.UI
{
    public abstract class TipController<TView, TModel> : UIController<TView, TModel> where TView : IuiView where TModel : IuiModel
    {

    }
}
