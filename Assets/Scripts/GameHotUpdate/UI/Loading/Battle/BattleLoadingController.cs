using GameHotUpdate.UI.MVC;

namespace GameHotUpdate.UI.Loading.Battle
{
    /// <summary>
    /// 
    /// </summary>
    public class BattleLoadingController : UIController<BattleLoadingView, BattleLoadingModel>
    {
        protected override System.Threading.Tasks.Task OnInit()
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void LoadBattle()
        {

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