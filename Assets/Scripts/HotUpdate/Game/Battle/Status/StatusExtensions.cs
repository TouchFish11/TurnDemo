namespace HotUpdate.Game.Battle.Status
{
    /// <summary>
    /// 状态拓展类
    /// </summary>
    public static class StatusExtensions
    {
        /// <summary>
        /// 是否是持续伤害
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool IsDot(this IStatus status)
        {
            return status is IDotStatus;
        }
    }
}
