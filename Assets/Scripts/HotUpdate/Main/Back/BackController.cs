using Core.UI.MVC;
using HotUpdate.Core.UI.MVC;

namespace HotUpdate.Main.Back
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 背景界面控制器
    /// </summary>
    public class BackController : UIController<BackView, BackModel>, IBackController
    {
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
    }
}