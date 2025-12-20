using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开始界面
/// </summary>
public class BeginView : UIView
{
    private GameObject progress;

    protected override void Awake()
    {
        base.Awake();

        progress = this.transform.Find(nameof(progress)).gameObject;
        progress.SetActive(false);
    }

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
        progress.SetActive(isShow);
    }
}
