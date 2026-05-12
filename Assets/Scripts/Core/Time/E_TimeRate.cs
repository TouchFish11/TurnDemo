
namespace Core.Time
{
    /// <summary>
    /// 时间缩放类型
    /// </summary>
    public enum E_TimeRate
    {
        /// <summary>
        /// 恢复到上次的设置的缩放
        /// </summary>
        Recovery = -1,
        /// <summary>
        /// 暂停
        /// </summary>
        Zero,
        /// <summary>
        /// 1倍速
        /// </summary>
        Normal,
        /// <summary>
        /// 二倍速
        /// </summary>
        Double,
    }
}
