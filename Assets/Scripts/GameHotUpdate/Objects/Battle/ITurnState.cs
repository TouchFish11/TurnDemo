namespace GameHotUpdate.Objects.Battle
{
    /// <summary>
    /// 回合状态接口
    /// </summary>
    public interface ITurnState
    {
        PlayerObject PlayerObject { get; }
        
        void Enter();
        
        void Exit();
    }
}
