using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合状态
    /// </summary>
    public abstract class TurnState : ITurnState
    {
        public RoleObject RoleObject { get; }

        protected TurnState(IBattleEntityObject battleEntity)
        {
            RoleObject = battleEntity as RoleObject;
        }
        
        public abstract void Enter();
        
        public abstract void Exit();
    }
}
