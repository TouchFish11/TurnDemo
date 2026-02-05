using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Event;

namespace GameHotUpdate.Battle.Event.Turn
{
    public class InsertCommandEvent : BattleEvent
    {
        public ICommand Command { get; }
        
        public InsertCommandEvent(IBattleContext context, ICommand command) : base(context)
        {
            Command = command;
        }
    }
}
