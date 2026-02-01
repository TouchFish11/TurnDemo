using Core.UI;
using Core.UI.MVC;
using TMPro;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Loading.Battle
{
    public class BattleLoadingView : UIView
    {
        [Inject] private Slider sliderLoading;
        [Inject] private TextMeshProUGUI txtLoading;

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="progress"></param>
        public void UpdateProgress(float progress)
        {
            sliderLoading.value = progress;
        }
    }
}
