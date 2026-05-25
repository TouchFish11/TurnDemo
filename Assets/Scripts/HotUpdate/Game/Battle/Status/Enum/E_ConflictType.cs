namespace HotUpdate.Game.Battle.Status.Enum
{
    /// <summary>
    /// 冲突类型
    /// </summary>
    public enum E_ConflictType : byte
    {
        /// <summary>
        /// 叠加
        /// </summary>
        Add = 1,
        /// <summary>
        /// 独立
        /// </summary>
        Lonely,
        /// <summary>
        /// 覆盖
        /// </summary>
        Cover,
    }
}