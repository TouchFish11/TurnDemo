using Framework;
using TMPro;
using UnityEngine.UI;

public class BattleLoadingView : UIView
{
    [Inject] private Slider sliderLoading;
    [Inject] private TextMeshProUGUI txtLoading;

    [System.Obsolete]
    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "progress":
                sliderLoading.value = (float)value;
                break;
            case "txtLoading":
                txtLoading.text = value.ToString();
                break;
        }
    }
}
