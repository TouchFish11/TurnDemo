using Framework;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class BattleLoadingView : UIView
{
    [Inject] private Slider sliderLoading;
    [Inject] private TextMeshProUGUI txtLoading;

    [System.Obsolete]
    public override void UpdateView(string key, object value)
    {

    }

    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="progress"></param>
    public void UpdateProgress(float progress)
    {
        sliderLoading.value = progress;
    }


}
