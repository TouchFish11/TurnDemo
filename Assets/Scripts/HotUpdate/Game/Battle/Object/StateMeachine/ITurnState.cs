using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object.StateMeachine
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
