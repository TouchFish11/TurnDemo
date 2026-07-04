namespace HotUpdate.Game.Battle.Context
{
    /// <summary>
    /// 战斗上下文拓展
    /// </summary>
    public static class BattleContextExtensions
    {
        /// <summary>
        /// 获取存活怪物数量
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetAliveMonsterEntityCount(this IBattleContext context)
        {
            var count = 0;
            foreach (var aliveMonsterEntity in context.GetAliveMonsterEntitys())
            {
                count++;
            }

            return count;
        }
        
        /// <summary>
        /// 获取存活玩家数量
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetAlivePlayerEntityCount(this IBattleContext context)
        {
            var count = 0;
            foreach (var aliveMonsterEntity in context.GetAlivePlayerEntitys())
            {
                count++;
            }

            return count;
        }
    }
}
