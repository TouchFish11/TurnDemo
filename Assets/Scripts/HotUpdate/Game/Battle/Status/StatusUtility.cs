using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Status
{
    /// <summary>
    /// 状态工具类
    /// </summary>
    public static class StatusUtility
    {
        /// <summary>
        /// 状态列表是否包含Dot
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        public static bool ContainDot(List<IStatus> statuses)
        {
            return statuses.FindIndex(status => status.IsDot()) != -1;
        }
    }
}
