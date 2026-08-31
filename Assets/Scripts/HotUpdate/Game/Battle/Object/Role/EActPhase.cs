namespace HotUpdate.Game.Battle.Object.Role
{
    public enum EActPhase
    {
        None,
        
        /// <summary>
        /// 回合开始阶段
        /// </summary>
        TurnStart,
            
        /// <summary>
        /// 回合进行中阶段
        /// </summary>
        Executing,
            
        /// <summary>
        /// 回合结束阶段
        /// </summary>
        TurnEnd,
            
    }
}
