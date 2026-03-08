using Core.UI.MVC;
using HotUpdate.Core.MVC;

namespace HotUpdate.Main.Loading.Battle
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 战斗加载界面控制器
    /// </summary>
    public class BattleLoadingController : UIController<BattleLoadingView, BattleLoadingModel>, IBattleLoadingController
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

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="progress"></param>
        public void UpdateProgress(float progress)
        {
            view.UpdateProgress(progress);
        }
    }
}