using HotUpdate.Base;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.Turn
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
