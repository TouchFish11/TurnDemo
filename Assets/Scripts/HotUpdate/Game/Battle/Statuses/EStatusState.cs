namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 状态生效时机类型
    /// </summary>
    public enum EStatusState
    {
        None = -1,
        
        /// <summary>
        /// 立刻生效，回合结束时会结算；
        /// </summary>
        Active,
        
        /// <summary>
        /// 下一回合生效，当前回合结束时不会结算；
        /// </summary>
        Pending,
    }
}
