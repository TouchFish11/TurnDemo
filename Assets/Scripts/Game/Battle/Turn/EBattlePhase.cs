namespace Game.Battle.Turn
{
    /// <summary>
    /// 战斗阶段
    /// </summary>
    public enum EBattlePhase : byte
    {
        None,
        
        /// <summary>
        /// 战斗准备阶段
        /// </summary>
        Preparation,
        
        /// <summary>
        /// 入场动画阶段
        /// </summary>
        EnterAnimation,
        
        /// <summary>
        /// 回合循环阶段
        /// </summary>
        TurnLoop,
        
        /// <summary>
        /// 战斗结束阶段
        /// </summary>
        Over,
    }
}
