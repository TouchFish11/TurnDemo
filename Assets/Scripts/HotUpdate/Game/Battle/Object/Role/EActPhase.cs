namespace HotUpdate.Game.Battle.Object.Role
{
    public enum EActPhase
    {
        None,
            
        /// <summary>
        /// Buff结算阶段
        /// </summary>
        SettlementBuff,
        
        /// <summary>
        /// 回合开始阶段
        /// </summary>
        TurnStart,
            
        /// <summary>
        /// 角色操作阶段
        /// </summary>
        Operator,
            
        /// <summary>
        /// 回合结束阶段
        /// </summary>
        TurnEnd,
            
    }
}
