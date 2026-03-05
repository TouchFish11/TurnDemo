using HotUpdate.Battle.Command;
using HotUpdate.Battle.Context;

namespace HotUpdate.Battle.Event.Turn
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
