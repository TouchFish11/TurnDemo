using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态格子UI
/// </summary>
public class StatusGridUI : BaseUIBehaviour
{
    private Image imgIcon;
    private Image imgBuffOrDeBuff;
    private TextMeshProUGUI txtPine;

    private IStatus status;

    private int currentPine;

    protected override void Awake()
    {
        base.Awake();

        imgIcon = binder.GetControl<Image>(nameof(imgIcon));
        imgBuffOrDeBuff = binder.GetControl<Image>(nameof(imgBuffOrDeBuff));
        txtPine = binder.GetControl<TextMeshProUGUI>(nameof(txtPine));
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="status"></param>
    public void Init(IStatus status)
    {
        this.status = status;
        this.currentPine = status.StatusProperty.CurrentPine;

        txtPine.text = status.StatusProperty.CurrentPine.ToString();
        ChangedBuffOrDeBuff();
    }

    private void ChangedBuffOrDeBuff()
    {
        if ((E_StatusType)status.StatusProperty.StatusInfo.f_statusType == E_StatusType.Positive)
        {
            imgBuffOrDeBuff.color = Color.blue;
        }
        else
        {
            imgBuffOrDeBuff.color = Color.red;
            imgBuffOrDeBuff.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    private void Update()
    {
        if (currentPine == status.StatusProperty.CurrentPine)
        {
            return;
        }

        txtPine.text = status.StatusProperty.CurrentPine.ToString();
        this.currentPine = status.StatusProperty.CurrentPine;
    }

    public int GetStatusId() => status.StatusProperty.StatusInfo.f_id;

    public bool IsValid => status.IsValid;
}
