using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主界面数据
/// </summary>
public class MainModel : UIModel
{
    // 交互UI缓存
    private readonly List<InteractUI> interactUIs = new List<InteractUI>();

    /// <summary>
    /// 设置交互
    /// </summary>
    /// <param name="interactUIs"></param>
    public void SetInteracts(List<InteractUI> interactUIs)
    {
        for (int i = 0; i < this.interactUIs.Count; i++)
        {
            PoolManager.Instance.PushObj(this.interactUIs[i].gameObject);
        }
        this.interactUIs.Clear();

        this.interactUIs.AddRange(interactUIs);
        TriggerDataChanged(nameof(interactUIs), interactUIs);
    }

    /// <summary>
    /// 激活交互UI
    /// </summary>
    public void ActiveInteract()
    {
        foreach (InteractUI interact in interactUIs)
        {
            interact.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 失活交互UI
    /// </summary>
    public void DeactivateInteract()
    {
        foreach (InteractUI interact in interactUIs)
        {
            interact.gameObject.SetActive(false);
        }
    }
}
