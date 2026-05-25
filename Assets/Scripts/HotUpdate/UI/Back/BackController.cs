using Core.UI.ViewController;
using HotUpdate.Game.Main.Back;

namespace HotUpdate.UI.Back
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 背景界面控制器
    /// </summary>
    public class BackController : UIController<BackView>
    {
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            return Task.CompletedTask;
        }
    }
}