using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态组件拓展
/// </summary>
public static class StatusComponentExtension
{
    /// <summary>
    /// 尝试获取状态
    /// </summary>
    /// <param name="statusId"></param>
    /// <param name="status"></param>
    /// <returns>不存在返回null</returns>
    public static bool TryGetStatus(this StatusComponent _, int statusId, List<IStatus> statuses, out IStatus status)
    {
        foreach (IStatus cacheStatus in statuses)
        {
            if (cacheStatus.StatusProperty.StatusInfo.f_id == statusId)
            {
                status = cacheStatus;
                return true;
            }
        }

        status = null;
        return false;
    }
}
