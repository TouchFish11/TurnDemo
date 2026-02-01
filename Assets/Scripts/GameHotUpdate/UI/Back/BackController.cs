using System;
using Core.Service;
using Core.UI;
using Game.UI.Back;
using GameHotUpdate.UI.MVC;

namespace GameHotUpdate.UI.Back
{
    /// <summary>
    /// 
    /// </summary>
    public class BackController : UIController<BackView, BackModel>, IBackController
    {
        public void CompletedHide(Action action)
        {
            action?.Invoke();
            ServiceLocator.Get<IUIManager>().DestroyView(this);
        }

        protected override System.Threading.Tasks.Task OnInit()
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}