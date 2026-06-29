namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 冲突类型
    /// </summary>
    public enum EConflictType : byte
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