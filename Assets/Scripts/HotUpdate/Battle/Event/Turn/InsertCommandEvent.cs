using HotUpdate.Battle.Command;
using HotUpdate.Battle.Context;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Command;
using HotUpdate.Core.Battle.Event;

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
