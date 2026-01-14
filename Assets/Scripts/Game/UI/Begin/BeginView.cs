using Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开始界面
/// </summary>
public class BeginView : UIView
{
    [Inject(1)] private RectTransform progress;

    protected override void Awake()
    {
        base.Awake();

        progress.gameObject.SetActive(false);
    }

    [System.Obsolete]
    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "sliderProgress":
                binder.GetControl<Slider>(key).value = (float)value;
                break;
            case "txtPro":
                binder.GetControl<Text>(key).text = value.ToString();
                break;
            case "txtPhase":
                binder.GetControl<Text>(key).text = value.ToString();
                break;
            case "txtSize":
                binder.GetControl<Text>(key).text = value.ToString();
                break;
            case "txtSpeed":
                binder.GetControl<Text>(key).text = value.ToString();
                break;
            case "isActiveProgress":
                ShowProgress((bool)value);
                break;
        }
    }

    /// <summary>
    /// 显示进度条
    /// </summary>
    private void ShowProgress(bool isShow)
    {
        progress.gameObject.SetActive(isShow);
    }
}
