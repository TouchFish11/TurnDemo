namespace GameHotUpdate.Objects.Battle
{
    /// <summary>
    /// 回合状态接口
    /// </summary>
    public interface ITurnState
    {
        BattleObject BattleEntity { get; }
        
        void Enter();

        void Execute();
        
        void Exit();
    }
}
