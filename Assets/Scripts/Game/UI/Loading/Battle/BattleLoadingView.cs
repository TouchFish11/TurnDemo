using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleLoadingView : UIView
{
    private Slider sliderLoading;
    private TextMeshProUGUI txtLoading;

    protected override void Awake()
    {
        base.Awake();

        sliderLoading = binder.GetControl<Slider>(nameof(sliderLoading));
        txtLoading = binder.GetControl<TextMeshProUGUI>(nameof(txtLoading));
    }

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
