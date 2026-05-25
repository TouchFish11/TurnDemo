using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Status
{
    /// <summary>
    /// 状态组件扩展方法
    /// </summary>
    public static class StatusComponentExtension
    {
        /// <summary>
        /// 尝试获取状态
        /// </summary>
        /// <param name="_">状态组件（扩展方法占位参数）</param>
        /// <param name="statusId">状态ID</param>
        /// <param name="statuses">状态列表</param>
        /// <param name="status">输出状态</param>
        /// <returns>不存在则返回null</returns>
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
}