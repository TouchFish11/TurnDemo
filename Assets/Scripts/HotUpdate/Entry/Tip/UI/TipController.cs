using Core.UI.MVC;

namespace HotUpdate.Entry.Tip.UI
{
    public abstract class TipController<TView, TModel> : UIController<TView, TModel> where TView : IuiView where TModel : IuiModel
    {

    }
}
