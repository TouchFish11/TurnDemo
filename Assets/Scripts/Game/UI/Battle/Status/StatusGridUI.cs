using Framework;
using Game.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态格子UI
/// </summary>
public class StatusGridUI : BaseUIBehaviour
{
    [Inject] private Image imgIcon;
    [Inject] private Image imgBuffOrDeBuff;
    [Inject] private TextMeshProUGUI txtPine;

    private IStatus status;
    private int currentPine;

    protected override void OnEnable()
    {
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
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

    private void OnUpdate()
    {
        if (currentPine == status.StatusProperty.CurrentPine)
        {
            return;
        }

        txtPine.text = status.StatusProperty.CurrentPine.ToString();
        this.currentPine = status.StatusProperty.CurrentPine;
    }

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }

    public int GetStatusId() => status.StatusProperty.StatusInfo.f_id;

    public bool IsValid => status.IsValid;
}
