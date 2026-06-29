using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Statuses
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
        public static bool ContainDot(IEnumerable<IStatus> statuses)
        {
            foreach (var statuse in statuses)
            {
                if (statuse.IsDot())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
