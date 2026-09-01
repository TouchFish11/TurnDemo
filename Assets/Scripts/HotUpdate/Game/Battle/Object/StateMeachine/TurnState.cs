using System.Threading.Tasks;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合状态
    /// </summary>
    public abstract class TurnState : ITurnState
    {
        public IBattleEntityObject BattleObject { get; }

        protected TurnState(IBattleEntityObject battleEntity)
        {
            BattleObject = battleEntity;
        }
        
        public abstract Task Enter();
        
        public abstract Task Exit();
    }
}
