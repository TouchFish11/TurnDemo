using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合状态
    /// </summary>
    public abstract class TurnState : ITurnState
    {
        public PlayerObject PlayerObject { get; }

        protected TurnState(IBattleEntityObject battleEntity)
        {
            PlayerObject = battleEntity as PlayerObject;
        }
        
        public abstract void Enter();
        
        public abstract void Exit();
    }
}
