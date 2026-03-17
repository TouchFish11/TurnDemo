using Core.UI.MVC;

namespace HotUpdate.Core.UI.MVC
{
    public interface IBattleLoadingController : IuiController
    {
        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="progress"></param>
        void UpdateProgress(float progress);
    }
}
