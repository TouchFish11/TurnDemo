using Core.UI;
using Core.UI.ViewController;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.Game.Main.Loading.Battle
{
    public class BattleLoadingView : UIView
    {
        [InjectUI] private Slider sliderLoading;
        [InjectUI] private TextMeshProUGUI txtLoading;

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
