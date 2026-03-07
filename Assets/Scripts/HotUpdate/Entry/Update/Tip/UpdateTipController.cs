using System;
using HotUpdate.Common;
using HotUpdate.Entry.Tip.UI;

namespace HotUpdate.Default.Update.Tip
{
    using Task = System.Threading.Tasks.Task;
    
    public class UpdateTipController : TipController<UpdateTipView, UpdateTipModel>
    {
        /// <summary>
        /// 确认事件
        /// </summary>
        public event Action OnSure;

        protected override Task OnShow()
        {
            return Task.CompletedTask;
        }

        protected override Task OnHide()
        {
            return Task.CompletedTask;
        }

        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        public void SetUpdateMessage(string message)
        {
            view.SetUpdateTip(message);
        }

        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="isActive">false则隐藏，tip忽略；true则显示tip的内容</param>
        /// <param name="tip">显示的文本</param>
        public void SetTipActive(bool isActive, string tip = "")
        {
            view.SetTipActive(isActive, tip);
        }

        protected override void ButtonOnClick(string btnName)
        {
            if (btnName == nameof(view.btnSure))
            {
                uiManager.DestroyView(AbKeyCollection.Ui, this);
                OnSure?.Invoke();
                OnSure = null;
            }
        }
    }
}
