using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 战技点UI
/// </summary>
public class BattlePointUI : BaseUIBehaviour
{
    private Image imgHas;

    protected override void Awake()
    {
        base.Awake();
        imgHas = binder.GetControl<Image>(nameof(imgHas));
    }

    /// <summary>
    /// 设置点活动状态
    /// </summary>
    /// <param name="active"></param>
    public void SetActivePoint(bool active)
    {
        imgHas.gameObject.SetActive(active);
    }
}
