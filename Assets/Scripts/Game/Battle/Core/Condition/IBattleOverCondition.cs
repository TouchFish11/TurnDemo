namespace Game.Battle
{
    /// <summary>
    /// 战斗结束条件
    /// </summary>
    public interface IBattleOverCondition
    {
        /// <summary>
        /// 检查结束
        /// </summary>
        /// <returns>true为结束</returns>
        bool CheckOver(IBattleContext context);
    }
}
