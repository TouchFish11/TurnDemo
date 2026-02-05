using Game.Battle.Objects;

namespace GameHotUpdate.Objects.Battle
{
    /// <summary>
    /// 回合状态
    /// </summary>
    public abstract class TurnState : ITurnState
    {
        public BattleObject BattleEntity { get; }

        protected TurnState(IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity as BattleObject;
        }
        
        public abstract void Enter();
        
        public abstract void Execute();
        
        public abstract void Exit();
    }
}
