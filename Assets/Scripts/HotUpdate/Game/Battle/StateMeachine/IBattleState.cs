using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.StateMeachine
{
    /// <summary>
    /// 战斗状态
    /// </summary>
    public interface IBattleState
    {
        public IBattleStateMachine BattleStateMachine { get; }
        
        public IBattleContext Context { get; }
        
        void Enter();
        
        void Exit();

        void Dispose();
    }
}
