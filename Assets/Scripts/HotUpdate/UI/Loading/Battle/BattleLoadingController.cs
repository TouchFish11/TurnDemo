using Core.UI.ViewController;
using HotUpdate.Base.UI;
using HotUpdate.Game.Main.Loading.Battle;

namespace HotUpdate.UI.Loading.Battle
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 战斗加载界面控制器
    /// </summary>
    public class BattleLoadingController : UIController<BattleLoadingView>, IBattleLoadingController
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