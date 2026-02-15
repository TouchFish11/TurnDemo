using System.Collections.Generic;
using Game.Battle.Status;

namespace GameHotUpdate.Battle.Status
{
    /// <summary>
    /// ״̬�����չ
    /// </summary>
    public static class StatusComponentExtension
    {
        /// <summary>
        /// ���Ի�ȡ״̬
        /// </summary>
        /// <param name="statusId"></param>
        /// <param name="status"></param>
        /// <returns>�����ڷ���null</returns>
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
