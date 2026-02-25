using System;
using System.Threading.Tasks;
using Core.Service;
using Core.UI;
using GameHotUpdate.Config;

namespace GameHotUpdate.Tip.UI.Update
{
    public class UpdateTipController : TipController<UpdateTipView, UpdateTipModel>
    {
        public event Action OnSure;
        
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        public void SetMessage(string message)
        {
            view.SetTip(message);
        }

        protected override void ButtonOnClick(string btnName)
        {
            if (btnName == nameof(view.btnSure))
            {
                ServiceLocator.Get<IUIManager>().DestroyView(AbKeyCollection.Ui, this);
                OnSure?.Invoke();
                OnSure = null;
            }
        }
    }
}
