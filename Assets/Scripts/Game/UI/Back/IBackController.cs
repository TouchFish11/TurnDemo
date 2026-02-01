using System;
using Core.UI.MVC;

namespace Game.UI.Back
{
    public interface IBackController : IuiController
    {
        void CompletedHide(Action action);
    }
}
