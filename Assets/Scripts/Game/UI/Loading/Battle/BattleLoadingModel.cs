using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLoadingModel : UIModel
{
    // 加载进度
    private float progress;
    // 加载文本
    private string txtLoading;

    public float Progress
    {
        get => progress;
        set
        {
            progress = value;
            TriggerDataChanged(nameof(progress), value);
        }
    }

    public string TxtLoadingText
    {
        get => txtLoading;
        set
        {
            txtLoading = value;
            TriggerDataChanged(nameof(txtLoading), value);
        }
    }
}
