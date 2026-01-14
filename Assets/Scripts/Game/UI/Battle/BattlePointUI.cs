using Framework;
using UnityEngine.UI;

/// <summary>
/// 战技点UI
/// </summary>
public class BattlePointUI : BaseUIBehaviour
{
    [Inject] private Image imgHas;

    /// <summary>
    /// 设置点活动状态
    /// </summary>
    /// <param name="active"></param>
    public void SetActivePoint(bool active)
    {
        imgHas.gameObject.SetActive(active);
    }
}
