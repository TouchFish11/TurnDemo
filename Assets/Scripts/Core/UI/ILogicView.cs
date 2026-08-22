namespace Core.UI
{
    public interface ILogicView<TView, in TLogic> where TView : ILogicView<TView, TLogic> where TLogic : IUILogic<TView, TLogic>
    {
        void Init(TLogic logic);
    }
}
