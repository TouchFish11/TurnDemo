namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 回合操作驱动：决定"打开操作"后实体要做什么。
    /// 角色 = 等待玩家输入（输入开启由 TurnStartEvent 处理器负责）；
    /// 怪物/AI = 立即决策并释放技能。
    /// </summary>
    public interface ITurnActionDriver
    {
        /// <summary>
        /// 打开操作（非阻塞）。由 TurnStartCommand 在结算完成后调用。
        /// </summary>
        void OnOperationOpened();
    }
}
