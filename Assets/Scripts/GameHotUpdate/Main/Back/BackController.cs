using Core.UI.MVC;

namespace GameHotUpdate.Main.Back
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 背景界面控制器
    /// </summary>
    public class BackController : UIController<BackView, BackModel>
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