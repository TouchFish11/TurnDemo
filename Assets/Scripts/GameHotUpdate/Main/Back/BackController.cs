using System;
using Core.Service;
using Core.UI;
using Core.UI.MVC;
using GameHotUpdate.Config;

namespace GameHotUpdate.Main.Back
{
    /// <summary>
    /// 
    /// </summary>
    public class BackController : UIController<BackView, BackModel>
    {
        public void CompletedHide(Action action)
        {
            action?.Invoke();
            ServiceLocator.Get<IUIManager>().DestroyView(AbKeyCollection.Ui, this);
        }

        protected override System.Threading.Tasks.Task OnInit()
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}