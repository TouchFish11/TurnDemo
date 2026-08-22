using System;

namespace Core.UI
{
    /// <summary>
    /// UI行为接口
    /// </summary>
    public interface IUILogic<out TView, TLogic> : IDisposable where TView : ILogicView<TView, TLogic> where TLogic : IUILogic<TView, TLogic>
    {
        TView View { get; }

        void OnEnable();
        
        void OnDisable();
    }
}
